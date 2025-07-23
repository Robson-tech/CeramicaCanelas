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
        alert("O campo 'Nome da Categoria' é obrigatório.");
        return;
    }

    const submitButton = form.querySelector('.submit-btn');
    const originalButtonHTML = submitButton.innerHTML;
    submitButton.disabled = true;
    submitButton.innerHTML = `<span class="loading-spinner"></span> Salvando...`;

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
            alert(`Erro: ${errorData.title || errorData.detail || 'Não foi possível salvar a categoria.'}`);
        }
    } catch (error) {
        alert("Erro de Conexão: Não foi possível comunicar com o servidor.");
    } finally {
        submitButton.disabled = false;
        submitButton.innerHTML = originalButtonHTML;
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

        // <<< INÍCIO DA NOVA LÓGICA >>>

        // Se a resposta for bem-sucedida (status 2xx), processa normalmente.
        if (response.ok) {
            const categories = await response.json();
            renderCategoryTable(categories, tableBody);
            return; // Encerra a função aqui.
        }

        // Se a resposta NÃO for bem-sucedida (status 4xx, 5xx), lemos o corpo do erro.
        const errorData = await response.json();

        // Verificamos se a mensagem de erro é a que esperamos para uma lista vazia.
        if (errorData && errorData.message === "Não há categórias cadastradas.") {
            // Se for, tratamos como um caso normal de lista vazia, sem exibir alerta.
            console.log("API informou que não há categorias. Renderizando tabela vazia.");
            renderCategoryTable([], tableBody);
        } else {
            // Se for qualquer outro erro, aí sim lançamos o erro para ser pego pelo catch.
            throw new Error(errorData.message || `Erro ${response.status}`);
        }
        // <<< FIM DA NOVA LÓGICA >>>

    } catch (error) {
        // Este bloco agora só captura erros de conexão ou erros inesperados da API.
        alert(`Erro ao Listar: ${error.message}`);
        tableBody.innerHTML = `<tr><td colspan="3" style="text-align: center; color: red;">${error.message}</td></tr>`;
    }
}

function renderCategoryTable(categories, tableBody) {
    tableBody.innerHTML = '';
    // Esta parte já estava correta, exibindo a mensagem para lista vazia.
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
    if (!confirm('Tem certeza que deseja excluir esta categoria? Esta ação não pode ser desfeita.')) return;

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
            const errorData = await response.json().catch(() => ({ title: "Erro ao Excluir", detail: "A categoria pode estar associada a produtos." }));
            // MODIFICADO: Trocado showErrorModal por alert
            alert(`Erro: ${errorData.title}\nDetalhe: ${errorData.detail}`);
        }
    } catch (error) {
        // MODIFICADO: Trocado showErrorModal por alert
        alert(`Erro de Conexão: ${error.message}`);
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

window.saveCategoryChanges = async (categoryId) => {
    const row = document.getElementById(`row-category-${categoryId}`);
    if (!row) return;

    const formData = new FormData();
    formData.append('Id', categoryId);
    formData.append('Name', row.querySelector('[name="Name"]').value);
    formData.append('Description', row.querySelector('[name="Description"]').value);

    if (!formData.get('Name')?.trim()) {
        alert('O nome da categoria não pode estar em branco.');
        return;
    }

    const saveButton = row.querySelector('.btn-save');
    saveButton.disabled = true;
    saveButton.innerHTML = `<span class="loading-spinner"></span>`;

    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/categories`, {
            method: 'PUT',
            headers: { 'Authorization': `Bearer ${accessToken}` },
            body: formData
        });

        if (response.ok) {
            // A atualização da tabela já é um feedback visual, o alert é opcional.
            // alert('Categoria atualizada com sucesso!');
            fetchAndRenderCategories();
        } else {
            const errorData = await response.json().catch(() => ({ title: "Erro ao Salvar" }));
            alert(`Erro: ${errorData.title || 'Não foi possível atualizar a categoria.'}`);
            // Em caso de erro, restaura a linha para que o usuário possa tentar novamente.
            cancelEditCategory(categoryId);
        }
    } catch (error) {
        alert(`Erro de Conexão: ${error.message}`);
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