console.log('Script js/lancamento-categoria.js DEFINIDO.');



// =======================================================
// INICIALIZAÇÃO DA PÁGINA
// =======================================================
function initDynamicForm() {
    console.log('▶️ initDynamicForm() de lancamento-categoria.js foi chamada.');
    
    initializeLaunchCategoryForm(document.querySelector('.launch-category-form'));
    initializeFilters();
    fetchAndRenderLaunchCategories(1);
}

function initializeFilters() {
    document.getElementById('filter-btn')?.addEventListener('click', () => fetchAndRenderLaunchCategories(1));
    document.getElementById('clear-filters-btn')?.addEventListener('click', () => {
        document.getElementById('search-input').value = '';
        document.getElementById('order-by').value = 'Name';
        fetchAndRenderLaunchCategories(1);
    });
}

// =======================================================
// LÓGICA DO FORMULÁRIO DE CADASTRO
// =======================================================
function initializeLaunchCategoryForm(form) {
    if (!form) return;
    form.onsubmit = (event) => {
        event.preventDefault();
        processAndSendLaunchCategoryData(form);
    };
}

async function processAndSendLaunchCategoryData(form) {
    const formData = new FormData(form);
    if (!formData.get('Name')?.trim()) {
        showErrorModal({ title: "Validação Falhou", detail: "O campo 'Nome da Categoria' é obrigatório." });
        return;
    }
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/financial/launch-categories`, {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${accessToken}` },
            body: formData,
        });
        if (response.ok) {
            alert('Categoria cadastrada com sucesso!');
            form.reset();
            fetchAndRenderLaunchCategories(1);
        } else {
            const errorData = await response.json();
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: "Não foi possível comunicar com o servidor." });
    }
}

// =======================================================
// LÓGICA DA TABELA (PAGINAÇÃO NO SERVIDOR)
// =======================================================
async function fetchAndRenderLaunchCategories(page = 1) {
    currentPage = page;
    const tableBody = document.querySelector('#launch-category-list-body');
    if (!tableBody) return;
    tableBody.innerHTML = '<tr><td colspan="2" style="text-align: center;">Buscando...</td></tr>';
    
    try {
        const accessToken = localStorage.getItem('accessToken');
        const search = document.getElementById('search-input')?.value;
        const orderBy = document.getElementById('order-by')?.value;

        const params = new URLSearchParams({ Page: currentPage, PageSize: 10, OrderBy: orderBy || 'Name' });
        if (search) params.append('Search', search);

        const url = `${API_BASE_URL}/financial/launch-categories/paged?${params.toString()}`;
        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error(`Falha ao buscar categorias (Status: ${response.status})`);
        
        const paginatedData = await response.json();
        renderLaunchCategoryTable(paginatedData.items, tableBody);
        renderPagination(paginatedData);
    } catch (error) {
        showErrorModal({ title: "Erro ao Listar", detail: error.message });
        tableBody.innerHTML = `<tr><td colspan="2" style="text-align: center; color: red;">${error.message}</td></tr>`;
        document.getElementById('pagination-controls').innerHTML = '';
    }
}

function renderLaunchCategoryTable(categories, tableBody) {
    tableBody.innerHTML = '';
    if (!categories || categories.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="2" style="text-align: center;">Nenhuma categoria de lançamento encontrada.</td></tr>';
        return;
    }
    categories.forEach(category => {
        const categoryJsonString = JSON.stringify(category).replace(/'/g, "&apos;");
        const rowHTML = `
            <tr id="row-launch-category-${category.id}">
                <td data-field="name">${category.name}</td>
                <td class="actions-cell" data-field="actions">
                    <button class="btn-action btn-edit" onclick='editLaunchCategory(${categoryJsonString})'>Editar</button>
                    <button class="btn-action btn-delete" onclick="deleteLaunchCategory('${category.id}')">Excluir</button>
                </td>
            </tr>`;
        tableBody.insertAdjacentHTML('beforeend', rowHTML);
    });
}

function renderPagination(paginationData) {
    const controlsContainer = document.getElementById('pagination-controls');
    if (!controlsContainer) return;
    controlsContainer.innerHTML = '';
    
    if (!paginationData || !paginationData.totalPages || paginationData.totalPages <= 1) return;
    
    const { page, totalPages } = paginationData;
    
    const prevButton = document.createElement('button');
    prevButton.textContent = 'Anterior';
    prevButton.className = 'pagination-btn';
    prevButton.disabled = page <= 1;
    prevButton.onclick = () => fetchAndRenderLaunchCategories(page - 1);
    
    const pageInfo = document.createElement('span');
    pageInfo.textContent = `Página ${page} de ${totalPages}`;
    pageInfo.className = 'pagination-info';
    
    const nextButton = document.createElement('button');
    nextButton.textContent = 'Próxima';
    nextButton.className = 'pagination-btn';
    nextButton.disabled = page >= totalPages;
    nextButton.onclick = () => fetchAndRenderLaunchCategories(page + 1);
    
    controlsContainer.appendChild(prevButton);
    controlsContainer.appendChild(pageInfo);
    controlsContainer.appendChild(nextButton);
}

// =======================================================
// FUNÇÕES DE CRUD (EDIÇÃO E EXCLUSÃO)
// =======================================================
window.deleteLaunchCategory = async (categoryId) => {
    if (!confirm('Tem certeza que deseja excluir esta categoria?')) return;
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/financial/launch-categories/${categoryId}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${accessToken}` }
        });
        if (response.ok) {
            alert('Categoria de lançamento excluída com sucesso!');
            fetchAndRenderLaunchCategories(currentPage);
        } else {
            const errorData = await response.json().catch(() => ({ title: "Erro ao Excluir" }));
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: error.message });
    }
};

window.editLaunchCategory = (category) => {
    const row = document.getElementById(`row-launch-category-${category.id}`);
    if (!row) return;
    originalRowHTML_LaunchCategory[category.id] = row.innerHTML;
    
    row.querySelector('[data-field="name"]').innerHTML = `<input type="text" name="Name" class="edit-input" value="${category.name}">`;
    
    row.querySelector('[data-field="actions"]').innerHTML = `
        <button class="btn-action btn-save" onclick="saveLaunchCategoryChanges('${category.id}')">Salvar</button>
        <button class="btn-action btn-cancel" onclick="cancelLaunchCategoryEdit('${category.id}')">Cancelar</button>
    `;
};

window.saveLaunchCategoryChanges = async (categoryId) => {
    const row = document.getElementById(`row-launch-category-${categoryId}`);
    if (!row) return;

    const formData = new FormData();
    formData.append('Id', categoryId);
    formData.append('Name', row.querySelector('[name="Name"]').value);

    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/financial/launch-categories`, {
            method: 'PUT',
            headers: { 'Authorization': `Bearer ${accessToken}` },
            body: formData
        });
        if (response.ok) {
            alert('Categoria de lançamento atualizada com sucesso!');
            fetchAndRenderLaunchCategories(currentPage);
        } else {
            const errorData = await response.json().catch(() => ({ title: "Erro ao Salvar" }));
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: error.message });
        cancelLaunchCategoryEdit(categoryId);
    }
};

window.cancelLaunchCategoryEdit = (categoryId) => {
    const row = document.getElementById(`row-launch-category-${categoryId}`);
    if (row && originalRowHTML_LaunchCategory[categoryId]) {
        row.innerHTML = originalRowHTML_LaunchCategory[categoryId];
        delete originalRowHTML_LaunchCategory[categoryId];
    }
};