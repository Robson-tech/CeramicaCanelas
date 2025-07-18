console.log('Script js/entrada.js DEFINIDO.');


// =======================================================
// INICIALIZAÇÃO DA PÁGINA
// =======================================================
function initDynamicForm() {
    console.log('▶️ initDynamicForm() de entrada.js foi chamada.');
    initializeFormListeners(document.querySelector('#productEntryForm'));
    initializeEntryTableFilters();
    loadProductCategories(document.querySelector('#entryCategoryFilter'), 'Todas as Categorias')
        .then(() => {
            fetchAndRenderEntries(1);
        });
}

// =======================================================
// LÓGICA DO FORMULÁRIO DE ENTRADA E MODAL
// =======================================================
function initializeFormListeners(form) {
    if (!form) return;
    const modal = document.getElementById('productSearchModal');
    const openModalBtn = document.getElementById('openProductModalBtn');
    const closeModalBtn = modal.querySelector('.modal-close-btn');

    openModalBtn.addEventListener('click', () => {
        modal.style.display = 'block';
        currentModalPage = 1;
        loadProductCategories(modal.querySelector('#modalCategoryFilter'), 'Todas as Categorias')
            .then(() => fetchAndRenderProductsInModal());
    });
    closeModalBtn.addEventListener('click', () => modal.style.display = 'none');
    window.addEventListener('click', (event) => {
        if (event.target === modal) modal.style.display = 'none';
    });
    
    modal.querySelector('#modalFilterBtn').addEventListener('click', () => {
        currentModalPage = 1;
        fetchAndRenderProductsInModal();
    });
    const clearModalFilterBtn = modal.querySelector('#modalClearFilterBtn');
    if (clearModalFilterBtn) {
        clearModalFilterBtn.addEventListener('click', () => {
            // Limpa os campos de filtro
            modal.querySelector('#modalSearchInput').value = '';
            modal.querySelector('#modalCategoryFilter').value = '';
            // Opcional: Reseta a ordenação para o padrão (primeira opção)
            modal.querySelector('#modalOrderBySelect').selectedIndex = 0; 
            
            // Reseta a página e busca novamente
            currentModalPage = 1;
            fetchAndRenderProductsInModal();
        });
    }
   

    initializeMainFormSubmit(form);
    initializeProductSelectionListener(modal);
}

async function fetchAndRenderProductsInModal() {
    const resultsContainer = document.getElementById('modalResultsContainer');
    resultsContainer.innerHTML = '<p>Buscando...</p>';
    try {
        const accessToken = localStorage.getItem('accessToken');
        if (!accessToken) throw new Error("Token não encontrado.");
        
        const search = document.getElementById('modalSearchInput').value;
        const categoryId = document.getElementById('modalCategoryFilter').value;
        const orderBy = document.getElementById('modalOrderBySelect').value;

        const params = new URLSearchParams({ Page: currentModalPage, PageSize: 10, OrderBy: orderBy });
        if (search) params.append('Search', search);
        if (categoryId) params.append('CategoryId', categoryId);
        
        const url = `${API_BASE_URL}/products/paged?${params.toString()}`;
        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error(`Falha na requisição: ${response.status}`);
        const paginatedData = await response.json();
        renderModalResults(paginatedData.items, resultsContainer);
        renderModalPagination(paginatedData);
    } catch (error) {
        resultsContainer.innerHTML = `<p style="color:red;">${error.message}</p>`;
    }
}

function renderModalResults(products, container) {
    if (!products || products.length === 0) { container.innerHTML = '<p>Nenhum produto encontrado.</p>'; return; }
    const table = document.createElement('table');
    table.className = 'results-table';
    table.innerHTML = `<thead><tr><th>Nome</th><th>Código</th><th>Estoque</th><th>Ação</th></tr></thead><tbody>
        ${products.map(p => `<tr><td>${p.name}</td><td>${p.code||'N/A'}</td><td>${p.stockCurrent||0}</td><td><button type="button" class="select-product-btn" data-id="${p.id}" data-name="${p.name}">Selecionar</button></td></tr>`).join('')}
    </tbody>`;
    container.innerHTML = '';
    container.appendChild(table);
}

function renderModalPagination(paginationData) {
    const controlsContainer = document.getElementById('modalPaginationControls');
    controlsContainer.innerHTML = '';
    if (paginationData.totalPages <= 1) return;
    const hasPreviousPage = paginationData.page > 1;
    const hasNextPage = paginationData.page < paginationData.totalPages;
    const prevButton = document.createElement('button');
    prevButton.textContent = 'Anterior';
    prevButton.className = 'pagination-btn';
    prevButton.disabled = !hasPreviousPage;
    prevButton.addEventListener('click', () => { currentModalPage--; fetchAndRenderProductsInModal(); });
    const pageInfo = document.createElement('span');
    pageInfo.textContent = `Página ${paginationData.page} de ${paginationData.totalPages}`;
    pageInfo.className = 'pagination-info';
    const nextButton = document.createElement('button');
    nextButton.textContent = 'Próxima';
    nextButton.className = 'pagination-btn';
    nextButton.disabled = !hasNextPage;
    nextButton.addEventListener('click', () => { currentModalPage++; fetchAndRenderProductsInModal(); });
    controlsContainer.appendChild(prevButton);
    controlsContainer.appendChild(pageInfo);
    controlsContainer.appendChild(nextButton);
}

function initializeProductSelectionListener(modal) {
    modal.querySelector('#modalResultsContainer').addEventListener('click', (event) => {
        if (event.target.classList.contains('select-product-btn')) {
            document.getElementById('selectedProductName').textContent = event.target.dataset.name;
            document.getElementById('productUuid').value = event.target.dataset.id;
            modal.style.display = 'none';
        }
    });
}

function initializeMainFormSubmit(form) {
    form.addEventListener('submit', async (event) => {
        event.preventDefault();
        // A função new FormData(form) pega automaticamente os campos com os nomes definidos no HTML
        // (ProductId, Quantity, UnitPrice), então não precisamos montar manualmente.
        const formData = new FormData(form);
        if (!formData.get('ProductId')) { showErrorModal({ title: "Validação Falhou", detail: "Por favor, busque e selecione um produto." }); return; }
        try {
            const accessToken = localStorage.getItem('accessToken');
            const response = await fetch(`${API_BASE_URL}/products-entry`, { method: 'POST', headers: { 'Authorization': `Bearer ${accessToken}` }, body: formData });
            if (response.ok) {
                alert('Entrada registrada com sucesso!');
                form.reset();
                document.getElementById('selectedProductName').textContent = 'Nenhum produto selecionado';
                fetchAndRenderEntries();
            } else {
                const errorData = await response.json();
                showErrorModal(errorData);
            }
        } catch (error) {
            showErrorModal({ title: "Erro de Conexão", detail: "Falha na comunicação com a API." });
        }
    });
}

// =======================================================
// LÓGICA DA TABELA DE ENTRADAS (LISTAGEM E CRUD)
// =======================================================
function initializeEntryTableFilters() {
    const filterBtn = document.getElementById('entryFilterBtn');
    const clearFilterBtn = document.getElementById('entryClearFilterBtn');
    filterBtn?.addEventListener('click', () => fetchAndRenderEntries(1));
    clearFilterBtn?.addEventListener('click', () => {
        document.getElementById('entrySearchInput').value = '';
        document.getElementById('entryCategoryFilter').value = '';
        fetchAndRenderEntries(1);
    });
}

async function fetchAndRenderEntries(page = 1) {
    currentEntryPage = page;
    const tableBody = document.querySelector('#entry-list-body');
    if (!tableBody) return;
    tableBody.innerHTML = '<tr><td colspan="5" style="text-align: center;">Buscando...</td></tr>';
    try {
        const accessToken = localStorage.getItem('accessToken');
        if (!accessToken) throw new Error("Não autenticado.");
        const search = document.getElementById('entrySearchInput')?.value;
        const categoryId = document.getElementById('entryCategoryFilter')?.value;
        const params = new URLSearchParams({ Page: currentEntryPage, PageSize: 10, OrderBy: 'EntryDate', Ascending: false });
        if (search) params.append('Search', search);
        if (categoryId) params.append('CategoryId', categoryId);
        const url = `${API_BASE_URL}/products-entry/paged?${params.toString()}`;
        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error(`Falha ao buscar as entradas (Status: ${response.status})`);
        const paginatedData = await response.json();
        renderEntryTable(paginatedData.items, tableBody);
        renderEntryPagination(paginatedData);
    } catch (error) {
        showErrorModal({ title: "Erro ao Listar Entradas", detail: error.message });
        tableBody.innerHTML = `<tr><td colspan="5" style="text-align: center; color: red;">Falha ao carregar.</td></tr>`;
        document.getElementById('entry-pagination-controls').innerHTML = '';
    }
}

function renderEntryTable(entries, tableBody) {
    tableBody.innerHTML = '';
    if (!entries || entries.length === 0) { tableBody.innerHTML = '<tr><td colspan="5" style="text-align: center;">Nenhuma entrada encontrada.</td></tr>'; return; }
    entries.forEach(entry => {
        const entryJsonString = JSON.stringify(entry).replace(/'/g, "&apos;");
        const formattedPrice = (entry.unitPrice || 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
        const formattedDate = new Date(entry.entryDate).toLocaleDateString('pt-BR');
        const rowHTML = `<tr id="row-entry-${entry.id}"><td data-field="productName">${entry.productName}</td><td data-field="quantity">${entry.quantity}</td><td data-field="unitPrice">${formattedPrice}</td><td data-field="entryDate">${formattedDate}</td><td class="actions-cell" data-field="actions"><button class="btn-action btn-edit" onclick='editEntry(${entryJsonString})'>Editar</button><button class="btn-action btn-delete" onclick="deleteEntry('${entry.id}')">Excluir</button></td></tr>`;
        tableBody.insertAdjacentHTML('beforeend', rowHTML);
    });
}

function renderEntryPagination(paginationData) {
    const controlsContainer = document.getElementById('entry-pagination-controls');
    controlsContainer.innerHTML = '';
    if (paginationData.totalPages <= 1) return;
    const hasPreviousPage = paginationData.page > 1;
    const hasNextPage = paginationData.page < paginationData.totalPages;
    const prevButton = document.createElement('button');
    prevButton.textContent = 'Anterior';
    prevButton.className = 'pagination-btn';
    prevButton.disabled = !hasPreviousPage;
    prevButton.addEventListener('click', () => fetchAndRenderEntries(currentEntryPage - 1));
    const pageInfo = document.createElement('span');
    pageInfo.textContent = `Página ${paginationData.page} de ${paginationData.totalPages}`;
    pageInfo.className = 'pagination-info';
    const nextButton = document.createElement('button');
    nextButton.textContent = 'Próxima';
    nextButton.className = 'pagination-btn';
    nextButton.disabled = !hasNextPage;
    nextButton.addEventListener('click', () => fetchAndRenderEntries(currentEntryPage + 1));
    controlsContainer.appendChild(prevButton);
    controlsContainer.appendChild(pageInfo);
    controlsContainer.appendChild(nextButton);
}

window.deleteEntry = async (entryId) => {
    if (!confirm('Tem certeza?')) return;
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/products-entry/${entryId}`, { method: 'DELETE', headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (response.ok) {
            alert('Entrada excluída!');
            fetchAndRenderEntries(currentEntryPage);
        } else {
            const errorData = await response.json().catch(() => ({ title: "Erro ao Excluir", detail: "Ocorreu um erro." }));
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: error.message });
    }
};

window.editEntry = (entry) => {
    const row = document.getElementById(`row-entry-${entry.id}`);
    if (!row) return;
    originalEntryRowHTML[entry.id] = row.innerHTML;
    row.querySelector('[data-field="quantity"]').innerHTML = `<input type="number" name="Quantity" class="edit-input" value="${entry.quantity}">`;
    row.querySelector('[data-field="unitPrice"]').innerHTML = `<input type="number" step="0.01" name="UnitPrice" class="edit-input" value="${entry.unitPrice}">`;
    row.querySelector('[data-field="actions"]').innerHTML = `<button class="btn-action btn-save" onclick="saveEntryChanges('${entry.id}')">Salvar</button><button class="btn-action btn-cancel" onclick="cancelEntryEdit('${entry.id}')">Cancelar</button>`;
};

window.saveEntryChanges = async (entryId) => {
    const row = document.getElementById(`row-entry-${entryId}`);
    if (!row) return;
    const payload = { id: entryId, quantity: parseInt(row.querySelector('[name="Quantity"]').value, 10), unitPrice: parseFloat(row.querySelector('[name="UnitPrice"]').value.replace(',', '.')) };
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/products-entry`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${accessToken}` },
            body: JSON.stringify(payload)
        });
        if (response.ok) {
            alert('Entrada atualizada!');
            fetchAndRenderEntries(currentEntryPage);
        } else {
            const errorData = await response.json().catch(() => ({ title: "Erro ao Salvar", detail: "Ocorreu um erro." }));
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: error.message });
        cancelEntryEdit(entryId);
    }
};

window.cancelEntryEdit = (entryId) => {
    const row = document.getElementById(`row-entry-${entryId}`);
    if (row && originalEntryRowHTML[entryId]) {
        row.innerHTML = originalEntryRowHTML[entryId];
        delete originalEntryRowHTML[entryId];
    }
};

async function loadProductCategories(selectElement, defaultOptionText = 'Selecione uma categoria') { 
    if (!selectElement) return; 
    try { 
        const accessToken = localStorage.getItem('accessToken'); 
        const response = await fetch(`${API_BASE_URL}/categories`, { headers: { 'Authorization': `Bearer ${accessToken}` } }); 
        if (!response.ok) throw new Error('Falha ao carregar categorias.'); 
        const categories = await response.json(); 
        selectElement.innerHTML = `<option value="">${defaultOptionText}</option>`; 
        categories.forEach(category => { 
            const option = new Option(category.name, category.id); 
            selectElement.appendChild(option); 
        }); 
    } catch (error) { 
        console.error('Erro ao carregar categorias:', error); 
        selectElement.innerHTML = '<option value="">Erro ao carregar</option>'; 
        throw error; 
    } 
}