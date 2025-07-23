console.log('Script js/entrada.js DEFINIDO.');

// =======================================================
// VARIÁVEIS GLOBAIS DE ESTADO
// =======================================================

// Assume que API_BASE_URL e showErrorModal são definidos em um script global
// const API_BASE_URL = 'http://localhost:5000/api'; 
// function showErrorModal(error) { ... }

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
// LÓGICA DO FORMULÁRIO PRINCIPAL E MODAIS
// =======================================================
function initializeFormListeners(form) {
    if (!form) return;
    
    // Configura a modal de Produtos
    const productModal = document.getElementById('productSearchModal');
    const openProductModalBtn = document.getElementById('openProductModalBtn');
    openProductModalBtn.addEventListener('click', () => {
        productModal.style.display = 'block';
        currentModalPage = 1;
        loadProductCategories(productModal.querySelector('#modalCategoryFilter'), 'Todas as Categorias')
            .then(() => fetchAndRenderProductsInModal());
    });
    productModal.querySelector('.modal-close-btn').addEventListener('click', () => productModal.style.display = 'none');
    productModal.querySelector('#modalFilterBtn').addEventListener('click', () => {
        currentModalPage = 1;
        fetchAndRenderProductsInModal();
    });
    initializeProductSelectionListener(productModal);

    // Configura a modal de Fornecedores
    const supplierModal = document.getElementById('supplierSearchModal');
    const openSupplierModalBtn = document.getElementById('openSupplierModalBtn');
    openSupplierModalBtn.addEventListener('click', () => {
        // Garante que não estamos em modo de edição ao abrir pelo formulário principal
        currentEditingEntryId = null; 
        supplierModal.style.display = 'block';
        currentSupplierModalPage = 1;
        fetchAndRenderSuppliersInModal();
    });
    supplierModal.querySelector('.modal-close-btn').addEventListener('click', () => supplierModal.style.display = 'none');
    supplierModal.querySelector('#modalSupplierFilterBtn').addEventListener('click', () => {
        currentSupplierModalPage = 1;
        fetchAndRenderSuppliersInModal();
    });
    initializeSupplierSelectionListener(supplierModal);

    // Fecha modais ao clicar fora
    window.addEventListener('click', (event) => {
        if (event.target === productModal) productModal.style.display = 'none';
        if (event.target === supplierModal) supplierModal.style.display = 'none';
    });

    initializeMainFormSubmit(form);
}

function initializeMainFormSubmit(form) {
    form.addEventListener('submit', async (event) => {
        event.preventDefault();
        const formData = new FormData(form);
        if (!formData.get('SupplierId')) {
            showErrorModal({ title: "Validação Falhou", detail: "Por favor, selecione um Fornecedor." });
            return;
        }
        if (!formData.get('ProductId')) {
            showErrorModal({ title: "Validação Falhou", detail: "Por favor, busque e selecione um Produto." });
            return;
        }

        const submitButton = form.querySelector('.submit-btn');
        const originalButtonHTML = submitButton.innerHTML;
        submitButton.disabled = true;
        submitButton.innerHTML = `<span class="loading-spinner"></span> Registrando...`;

        try {
            const accessToken = localStorage.getItem('accessToken');
            const response = await fetch(`${API_BASE_URL}/products-entry`, { method: 'POST', headers: { 'Authorization': `Bearer ${accessToken}` }, body: formData });
            if (response.ok) {
                alert('Entrada registrada com sucesso!');
                form.reset();
                document.getElementById('selectedProductName').textContent = 'Nenhum produto selecionado';
                document.getElementById('selectedSupplierName').textContent = 'Nenhum fornecedor selecionado';
                fetchAndRenderEntries(1);
            } else {
                const errorData = await response.json();
                showErrorModal(errorData);
            }
        } catch (error) {
            showErrorModal({ title: "Erro de Conexão", detail: "Falha na comunicação com a API." });
        } finally {
            submitButton.disabled = false;
            submitButton.innerHTML = originalButtonHTML;
        }
    });
}
// =======================================================
// LÓGICA DA MODAL DE BUSCA DE PRODUTOS
// =======================================================
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
        renderModalResults(paginatedData.items, resultsContainer, 'product');
        renderModalPagination(paginatedData, 'product');
    } catch (error) {
        resultsContainer.innerHTML = `<p style="color:red;">${error.message}</p>`;
        document.getElementById('modalPaginationControls').innerHTML = '';
    }
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

// =======================================================
// LÓGICA DA MODAL DE BUSCA DE FORNECEDORES
// =======================================================
async function fetchAndRenderSuppliersInModal() {
    const resultsContainer = document.getElementById('modalSupplierResultsContainer');
    resultsContainer.innerHTML = '<p>Buscando fornecedores...</p>';
    try {
        const accessToken = localStorage.getItem('accessToken');
        if (!accessToken) throw new Error("Token não encontrado.");
        
        const search = document.getElementById('modalSupplierSearchInput').value;
        const params = new URLSearchParams({ Page: currentSupplierModalPage, PageSize: 10, OrderBy: 'Name' });
        if (search) params.append('Search', search);
        
        const url = `${API_BASE_URL}/supplier/paged?${params.toString()}`;
        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error(`Falha na requisição: ${response.status}`);
        
        const paginatedData = await response.json();
        renderModalResults(paginatedData.items, resultsContainer, 'supplier');
        renderModalPagination(paginatedData, 'supplier');
    } catch (error) {
        resultsContainer.innerHTML = `<p style="color:red;">${error.message}</p>`;
        document.getElementById('modalSupplierPaginationControls').innerHTML = '';
    }
}

function initializeSupplierSelectionListener(modal) {
    modal.querySelector('#modalSupplierResultsContainer').addEventListener('click', (event) => {
        if (event.target.classList.contains('select-supplier-btn')) {
            const supplierId = event.target.dataset.id;
            const supplierName = event.target.dataset.name;

            // ✅ LÓGICA CORRIGIDA: Verifica se está editando ou adicionando novo
            if (currentEditingEntryId) {
                // Atualiza os campos na linha da tabela que está sendo editada
                const row = document.getElementById(`row-entry-${currentEditingEntryId}`);
                if (row) {
                    row.querySelector('.edit-supplier-name-span').textContent = supplierName;
                    row.querySelector('.edit-supplier-id-input').value = supplierId;
                }
                currentEditingEntryId = null; // Reseta o estado de edição
            } else {
                // Comportamento original: atualiza o formulário principal
                document.getElementById('selectedSupplierName').textContent = supplierName;
                document.getElementById('supplierUuid').value = supplierId;
            }
            
            modal.style.display = 'none'; // Fecha a modal em ambos os casos
        }
    });
}

// =======================================================
// FUNÇÕES GENÉRICAS PARA MODAIS
// =======================================================
function renderModalResults(items, container, type) {
    if (!items || items.length === 0) { container.innerHTML = `<p>Nenhum ${type === 'product' ? 'produto' : 'fornecedor'} encontrado.</p>`; return; }
    const table = document.createElement('table');
    table.className = 'results-table';
    if (type === 'product') {
        table.innerHTML = `<thead><tr><th>Nome</th><th>Código</th><th>Estoque</th><th>Ação</th></tr></thead><tbody>
            ${items.map(p => `<tr><td>${p.name}</td><td>${p.code||'N/A'}</td><td>${p.stockCurrent||0}</td><td><button type="button" class="select-product-btn" data-id="${p.id}" data-name="${p.name}">Selecionar</button></td></tr>`).join('')}
        </tbody>`;
    } else { // type === 'supplier'
        table.innerHTML = `<thead><tr><th>Nome</th><th>CNPJ</th><th>Telefone</th><th>Ação</th></tr></thead><tbody>
            ${items.map(s => `<tr><td>${s.name}</td><td>${s.cnpj||'N/A'}</td><td>${s.phone||'N/A'}</td><td><button type="button" class="select-supplier-btn" data-id="${s.id}" data-name="${s.name}">Selecionar</button></td></tr>`).join('')}
        </tbody>`;
    }
    container.innerHTML = '';
    container.appendChild(table);
}

function renderModalPagination(paginationData, type) {
    const controlsContainer = (type === 'product') 
        ? document.getElementById('modalPaginationControls') 
        : document.getElementById('modalSupplierPaginationControls');
    
    controlsContainer.innerHTML = '';
    if (paginationData.totalPages <= 1) return;
    const { page, totalPages } = paginationData;
    const hasPreviousPage = page > 1;
    const hasNextPage = page < totalPages;
    const prevButton = `<button class="pagination-btn" ${!hasPreviousPage ? 'disabled' : ''} data-page="${page - 1}" data-type="${type}">Anterior</button>`;
    const nextButton = `<button class="pagination-btn" ${!hasNextPage ? 'disabled' : ''} data-page="${page + 1}" data-type="${type}">Próxima</button>`;
    const pageInfo = `<span class="pagination-info">Página ${page} de ${totalPages}</span>`;
    
    controlsContainer.innerHTML = prevButton + pageInfo + nextButton;

    controlsContainer.querySelectorAll('.pagination-btn').forEach(button => {
        button.addEventListener('click', (e) => {
            const target = e.currentTarget;
            if (target.dataset.type === 'product') {
                currentModalPage = parseInt(target.dataset.page);
                fetchAndRenderProductsInModal();
            } else {
                currentSupplierModalPage = parseInt(target.dataset.page);
                fetchAndRenderSuppliersInModal();
            }
        });
    });
}


// =======================================================
// LÓGICA DA TABELA DE ENTRADAS (LISTAGEM E CRUD)
// =======================================================
function initializeEntryTableFilters() {
    document.getElementById('entryFilterBtn')?.addEventListener('click', () => fetchAndRenderEntries(1));
    document.getElementById('entryClearFilterBtn')?.addEventListener('click', () => {
        document.getElementById('entrySearchInput').value = '';
        document.getElementById('entryCategoryFilter').value = '';
        fetchAndRenderEntries(1);
    });
}

async function fetchAndRenderEntries(page = 1) {
    currentEntryPage = page;
    const tableBody = document.querySelector('#entry-list-body');
    if (!tableBody) return;
    // ✅ Colspan corrigido para 7, que agora corresponde ao número de cabeçalhos
    tableBody.innerHTML = '<tr><td colspan="7" style="text-align: center;">Buscando...</td></tr>';
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
        tableBody.innerHTML = `<tr><td colspan="7" style="text-align: center; color: red;">Falha ao carregar.</td></tr>`;
        document.getElementById('entry-pagination-controls').innerHTML = '';
    }
}

function renderEntryTable(entries, tableBody) {
    tableBody.innerHTML = '';
    // ✅ Colspan corrigido para 7
    if (!entries || entries.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="7" style="text-align: center;">Nenhuma entrada encontrada.</td></tr>';
        return;
    }
    entries.forEach(entry => {
        const entryJsonString = JSON.stringify(entry).replace(/'/g, "&apos;");
        const formattedPrice = (entry.unitPrice || 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
        const formattedDate = new Date(entry.entryDate).toLocaleDateString('pt-BR');
        // ✅ A ordem das colunas agora bate com o thead do HTML
        const rowHTML = `
            <tr id="row-entry-${entry.id}">
                <td data-field="productName">${entry.productName}</td>
                <td data-field="supplierName">${entry.supplierName || 'N/A'}</td>
                <td data-field="quantity">${entry.quantity}</td>
                <td data-field="unitPrice">${formattedPrice}</td>
                <td data-field="entryDate">${formattedDate}</td>
                <td data-field="insertedBy">${entry.insertedBy || 'N/A'}</td>
                <td class="actions-cell" data-field="actions">
                    <button class="btn-action btn-edit" onclick='editEntry(${entryJsonString})'>Editar</button>
                    <button class="btn-action btn-delete" onclick="deleteEntry('${entry.id}')">Excluir</button>
                </td>
            </tr>`;
        tableBody.insertAdjacentHTML('beforeend', rowHTML);
    });
}

function renderEntryPagination(paginationData) {
    const controlsContainer = document.getElementById('entry-pagination-controls');
    if (!controlsContainer) return;
    controlsContainer.innerHTML = '';
    if (paginationData.totalPages <= 1) return;
    const { page, totalPages } = paginationData;
    const hasPreviousPage = page > 1;
    const hasNextPage = page < totalPages;
    const prevButton = `<button class="pagination-btn" ${!hasPreviousPage ? 'disabled' : ''} onclick="fetchAndRenderEntries(${page - 1})">Anterior</button>`;
    const nextButton = `<button class="pagination-btn" ${!hasNextPage ? 'disabled' : ''} onclick="fetchAndRenderEntries(${page + 1})">Próxima</button>`;
    const pageInfo = `<span class="pagination-info">Página ${page} de ${totalPages}</span>`;
    controlsContainer.innerHTML = prevButton + pageInfo + nextButton;
}

window.deleteEntry = async (entryId) => {
    if (!confirm('Tem certeza que deseja excluir esta entrada? A ação não pode ser desfeita.')) return;
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/products-entry/${entryId}`, { method: 'DELETE', headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (response.ok) {
            alert('Entrada excluída com sucesso!');
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

    const anotherEditingRow = document.querySelector('.btn-save');
    if (anotherEditingRow) {
        cancelEntryEdit(anotherEditingRow.closest('tr').id.replace('row-entry-', ''));
    }

    originalEntryRowHTML[entry.id] = row.innerHTML;
    
    // ✅ CORREÇÃO: Adicionado um input escondido para productId, necessário para a atualização.
    // O nome do fornecedor é atualizado via modal, por isso tem um span e um botão.
    const editRowContent = `
        <td data-field="productName">
            ${entry.productName}
            <input type="hidden" class="edit-product-id-input" value="${entry.productId}">
        </td>
        <td data-field="supplierName">
            <span class="edit-supplier-name-span">${entry.supplierName || 'N/A'}</span>
            <input type="hidden" class="edit-supplier-id-input" value="${entry.supplierId}">
            <button type="button" class="btn-action btn-edit-inline" 
                    onclick="currentEditingEntryId='${entry.id}'; document.getElementById('supplierSearchModal').style.display='block'; fetchAndRenderSuppliersInModal();">
                Alterar
            </button>
        </td>
        <td data-field="quantity">
            <input type="number" name="Quantity" class="edit-input" value="${entry.quantity}" min="1" step="1">
        </td>
        <td data-field="unitPrice">
            <input type="number" step="0.01" name="UnitPrice" class="edit-input" value="${entry.unitPrice}" min="0">
        </td>
        <td data-field="entryDate">${new Date(entry.entryDate).toLocaleDateString('pt-BR')}</td>
        <td data-field="insertedBy">${entry.insertedBy || 'N/A'}</td>
        <td class="actions-cell" data-field="actions">
            <button class="btn-action btn-save" onclick="saveEntryChanges('${entry.id}')">Salvar</button>
            <button class="btn-action btn-cancel" onclick="cancelEntryEdit('${entry.id}')">Cancelar</button>
        </td>
    `;
    row.innerHTML = editRowContent;
};

window.saveEntryChanges = async (entryId) => {
    console.log(`▶️ Iniciando saveEntryChanges para o ID: ${entryId}`);
    const row = document.getElementById(`row-entry-${entryId}`);
    if (!row) {
        console.error(`❌ Erro crítico: A linha da tabela com id 'row-entry-${entryId}' não foi encontrada.`);
        return;
    }

    const rawSupplierId = row.querySelector('.edit-supplier-id-input').value;
    const rawQuantity = row.querySelector('[name="Quantity"]').value;
    const rawUnitPrice = row.querySelector('[name="UnitPrice"]').value;

    if (!rawSupplierId || rawQuantity <= 0 || rawUnitPrice < 0) {
        alert('Por favor, verifique se todos os campos estão preenchidos corretamente.');
        return;
    }

    const saveButton = row.querySelector('.btn-save');
    saveButton.disabled = true;
    saveButton.innerHTML = `<span class="loading-spinner"></span>`;

    const formData = new FormData();
    formData.append('Id', entryId);
    formData.append('SupplierId', rawSupplierId);
    formData.append('Quantity', rawQuantity);
    formData.append('UnitPrice', rawUnitPrice.replace(',', '.'));

    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/products-entry`, {
            method: 'PUT',
            headers: { 'Authorization': `Bearer ${accessToken}` },
            body: formData
        });

        if (response.ok) {
            delete originalEntryRowHTML[entryId];
            fetchAndRenderEntries(currentEntryPage);
        } else {
            const errorData = await response.json().catch(() => ({ title: "Erro na resposta", detail: "A API não retornou um JSON válido." }));
            showErrorModal(errorData);
            cancelEntryEdit(entryId); // Restaura a linha em caso de erro
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
    // Reseta a variável de estado de edição
    currentEditingEntryId = null;
};

// =======================================================
// FUNÇÕES UTILITÁRIAS
// =======================================================
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