console.log('Script js/category.js DEFINIDO.');


// =======================================================
// INICIALIZAÇÃO DA PÁGINA
// =======================================================
function initDynamicForm() {
    console.log('▶️ initDynamicForm() de category.js foi chamada.');
    initializeCategoryForm(document.querySelector('.category-form'));
    fetchAndRenderCategories();
}

// =======================================================
// LÓGICA DO FORMULÁRIO DE CADASTRO
// =======================================================
function initializeCategoryForm(form) {
    if (!form) return;
    form.onsubmit = (event) => {
        event.preventDefault();
        processAndSendCategoryData(form);
    };
}

async function processAndSendCategoryData(form) {
    const formData = new FormData(form);
    if (!formData.get('Name')?.trim()) {
        showErrorModal({ title: "Validação Falhou", detail: "O campo 'Nome da Categoria' é obrigatório." });
        return;
    }

    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/categories`, {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${accessToken}` },
            body: formData,
        });
        if (response.ok) {
            alert('Categoria cadastrada com sucesso!');
            form.reset();
            fetchAndRenderCategories();
        } else {
            const errorData = await response.json();
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: "Não foi possível comunicar com o servidor." });
    }
}

// =======================================================
// LÓGICA DA TABELA (LISTAGEM E CRUD)
// =======================================================
async function fetchAndRenderCategories() {
    const tableBody = document.querySelector('#category-list-body');
    if (!tableBody) return;
    tableBody.innerHTML = '<tr><td colspan="3" style="text-align: center;">Buscando...</td></tr>';
    
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/categories`, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error('Falha ao buscar a lista de categorias.');
        const categories = await response.json();
        renderCategoryTable(categories, tableBody);
    } catch (error) {
        showErrorModal({ title: "Erro ao Listar", detail: error.message });
        tableBody.innerHTML = `<tr><td colspan="3" style="text-align: center; color: red;">${error.message}</td></tr>`;
    }
}

function renderCategoryTable(categories, tableBody) {
    tableBody.innerHTML = '';
    if (!categories || categories.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="3" style="text-align: center;">Nenhuma categoria cadastrada.</td></tr>';
        return;
    }
    categories.forEach(category => {
        const categoryJsonString = JSON.stringify(category).replace(/'/g, "&apos;");
        const rowHTML = `
            <tr id="row-category-${category.id}">
                <td data-field="name">${category.name}</td>
                <td data-field="description">${category.description || ''}</td>
                <td class="actions-cell" data-field="actions">
                    <button class="btn-action btn-edit" onclick='editCategory(${categoryJsonString})'>Editar</button>
                    <button class="btn-action btn-delete" onclick="deleteCategory('${category.id}')">Excluir</button>
                </td>
            </tr>`;
        tableBody.insertAdjacentHTML('beforeend', rowHTML);
    });
}

window.deleteCategory = async (categoryId) => {
    if (!confirm('Tem certeza que deseja excluir esta categoria?')) return;
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/categories/${categoryId}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${accessToken}` }
        });
        if (response.ok) {
            alert('Categoria excluída com sucesso!');
            fetchAndRenderCategories();
        } else {
            const errorData = await response.json().catch(() => ({ title: "Erro ao Excluir" }));
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: error.message });
    }
};

window.editCategory = (category) => {
    const row = document.getElementById(`row-category-${category.id}`);
    if (!row) return;
    originalRowHTML_Category[category.id] = row.innerHTML;
    
    row.querySelector('[data-field="name"]').innerHTML = `<input type="text" name="Name" class="edit-input" value="${category.name}">`;
    row.querySelector('[data-field="description"]').innerHTML = `<textarea name="Description" class="edit-input">${category.description || ''}</textarea>`;
    
    row.querySelector('[data-field="actions"]').innerHTML = `
        <button class="btn-action btn-save" onclick="saveCategoryChanges('${category.id}')">Salvar</button>
        <button class="btn-action btn-cancel" onclick="cancelEditCategory('${category.id}')">Cancelar</button>
    `;
};

/**
 * Salva as alterações de uma categoria.
 * ATUALIZADO: Envia os dados como multipart/form-data.
 */
window.saveCategoryChanges = async (categoryId) => {
    const row = document.getElementById(`row-category-${categoryId}`);
    if (!row) return;

    // 1. Cria um objeto FormData
    const formData = new FormData();

    // 2. Adiciona os campos com os nomes que a API espera
    formData.append('Id', categoryId);
    formData.append('Name', row.querySelector('[name="Name"]').value);
    formData.append('Description', row.querySelector('[name="Description"]').value);

    // Validação
    if (!formData.get('Name') || formData.get('Name').trim() === '') {
        alert('O nome da categoria não pode estar em branco.');
        return;
    }

    try {
        const accessToken = localStorage.getItem('accessToken');
        
        // 3. Faz a requisição PUT, mas sem o 'Content-Type' e com o body sendo o formData
        const response = await fetch(`${API_BASE_URL}/categories`, {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${accessToken}`
            },
            body: formData
        });

        if (response.ok) {
            alert('Categoria atualizada com sucesso!');
            fetchAndRenderCategories();
        } else {
            const errorData = await response.json().catch(() => ({ title: "Erro ao Salvar" }));
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: error.message });
        cancelEditCategory(categoryId);
    }
};

window.cancelEditCategory = (categoryId) => {
    const row = document.getElementById(`row-category-${categoryId}`);
    if (row && originalRowHTML_Category[categoryId]) {
        row.innerHTML = originalRowHTML_Category[categoryId];
        delete originalRowHTML_Category[categoryId];
    }
};