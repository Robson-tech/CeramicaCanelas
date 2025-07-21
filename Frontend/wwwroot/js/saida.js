console.log('Script js/saida.js DEFINIDO.');

// Este script utiliza as variáveis globais de main.js
// (API_BASE_URL, showErrorModal, positionMap, getPositionName, etc.)



// =======================================================
// INICIALIZAÇÃO PRINCIPAL
// =======================================================

function initDynamicForm() {
    console.log('▶️ initDynamicForm() de saida.js foi chamada.');
    initializeExitForm();
    initializeProductModal();
    initializeEmployeeModal();
    initializeHistoryFilters();
    // A função loadProductCategories não é necessária para o filtro de histórico aqui,
    // mas pode ser adicionada se o filtro de categoria for reintroduzido no HTML
    fetchAndRenderHistory(1);
}

// =======================================================
// LÓGICA DO FORMULÁRIO DE SAÍDA (CARRINHO)
// =======================================================

function initializeExitForm() {
    const exitForm = document.getElementById('productExitForm');
    if (!exitForm) return;

    exitForm.addEventListener('submit', handleFormSubmit);
    
    document.getElementById('exitItemsTbody')?.addEventListener('click', (event) => {
        if (event.target.classList.contains('btn-delete')) {
            event.target.closest('tr').remove();
            checkPlaceholder();
        }
    });
}

async function handleFormSubmit(event) {
    event.preventDefault();
    
    const employeeId = document.getElementById('employeeId')?.value;
    const observation = document.getElementById('observation')?.value;
    const itemRows = document.querySelectorAll('#exitItemsTbody tr:not(#placeholder-row)');

    if (!employeeId) {
        showErrorModal({ title: "Validação Falhou", detail: "Por favor, selecione um funcionário responsável." });
        return;
    }
    if (itemRows.length === 0) {
        showErrorModal({ title: "Validação Falhou", detail: "Adicione pelo menos um produto à lista de saída." });
        return;
    }

    const exitItems = [];
    for (const row of itemRows) {
        const quantityInput = row.querySelector('input[type="number"]');
        const quantity = quantityInput ? quantityInput.value : 0;
        if (!quantity || parseInt(quantity) <= 0) {
            showErrorModal({ title: "Validação Falhou", detail: `Por favor, insira uma quantidade válida para o produto "${row.dataset.productName}".` });
            return;
        }
        exitItems.push({
            ProductId: row.dataset.productId,
            Quantity: parseInt(quantity),
            IsReturnable: row.querySelector('input[type="checkbox"]').checked,
        });
    }

    const requests = exitItems.map(item => {
        const formData = new FormData();
        formData.append('ProductId', item.ProductId);
        formData.append('EmployeeId', employeeId);
        formData.append('Quantity', item.Quantity);
        formData.append('IsReturnable', item.IsReturnable);
        formData.append('Observation', observation);
        return sendExitRequest(formData);
    });

    try {
        await Promise.all(requests);
        alert('Todas as saídas foram registradas com sucesso!');
        document.getElementById('productExitForm').reset();
        document.getElementById('exitItemsTbody').innerHTML = '<tr id="placeholder-row"><td colspan="4" style="text-align: center; color: #888;">Nenhum produto adicionado.</td></tr>';
        document.getElementById('selectedEmployeeName').textContent = 'Nenhum funcionário selecionado';
        fetchAndRenderHistory(1);
    } catch (error) {
        console.error("Falha ao registrar uma ou mais saídas:", error);
        showErrorModal({ title: "Erro ao Registrar Saídas", detail: error.message });
    }
}

async function sendExitRequest(formData) {
    const accessToken = localStorage.getItem('accessToken');
    const response = await fetch(`${API_BASE_URL}/products-exit`, {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${accessToken}` },
        body: formData
    });
    if (!response.ok) {
        const errorData = await response.json().catch(() => null);
        throw new Error(errorData?.message || `Erro ${response.status}`);
    }
    return true;
}

function addProductToExitTable(product) {
    const tbody = document.getElementById('exitItemsTbody');
    if (!tbody) return;
    if (document.querySelector(`tr[data-product-id="${product.id}"]`)) {
        alert('Este produto já foi adicionado à lista.');
        return;
    }
    checkPlaceholder();
    const newRow = document.createElement('tr');
    newRow.dataset.productId = product.id;
    newRow.dataset.productName = product.name;
    newRow.innerHTML = `
        <td>${product.name}</td>
        <td><input type="number" class="form-input" value="1" min="1"></td>
        <td><input type="checkbox" class="form-checkbox"></td>
        <td><button type="button" class="btn-action btn-delete">Remover</button></td>
    `;
    tbody.appendChild(newRow);
}

function checkPlaceholder() {
    const tbody = document.getElementById('exitItemsTbody');
    if(!tbody) return;
    const placeholder = document.getElementById('placeholder-row');
    if (placeholder && tbody.querySelectorAll('tr:not(#placeholder-row)').length > 0) {
        placeholder.remove();
    } else if (!placeholder && tbody.children.length === 0) {
        tbody.innerHTML = '<tr id="placeholder-row"><td colspan="4" style="text-align: center; color: #888;">Nenhum produto adicionado.</td></tr>';
    }
}


// =======================================================
// LÓGICA DA MODAL DE BUSCA DE PRODUTOS
// =======================================================
function initializeProductModal() {
    const modal = document.getElementById('productSearchModal');
    const openBtn = document.getElementById('openProductModalBtn');
    if (!modal || !openBtn) return;
    const closeBtn = modal.querySelector('.modal-close-btn');
    
    openBtn.addEventListener('click', () => { 
        currentProductModalPage = 1;
        modal.style.display = 'block'; 
        fetchPaginatedProducts(currentProductModalPage); 
    });
    
    if(closeBtn) closeBtn.addEventListener('click', () => { modal.style.display = 'none'; });
    window.addEventListener('click', (event) => { if (event.target === modal) modal.style.display = 'none'; });
    
    modal.querySelector('#modalProductResultsContainer')?.addEventListener('click', (event) => {
        if (event.target.classList.contains('btn-select-product')) {
            const product = JSON.parse(event.target.dataset.product);
            addProductToExitTable(product);
            modal.style.display = 'none';
        }
    });
}

async function fetchPaginatedProducts(page) {
    currentProductModalPage = page;
    const resultsContainer = document.getElementById('modalProductResultsContainer');
    const paginationContainer = document.getElementById('modalProductPaginationControls');
    if(!resultsContainer || !paginationContainer) return;

    resultsContainer.innerHTML = '<p>Buscando...</p>';
    paginationContainer.innerHTML = '';
    
    const params = new URLSearchParams({ Page: page, PageSize: 5 });
    const url = `${API_BASE_URL}/products/paged?${params.toString()}`;
    
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error('Falha ao buscar produtos.');
        
        const data = await response.json();
        const tableHTML = (data.items && data.items.length > 0) 
            ? `<table class="results-table"><thead><tr><th>Nome</th><th>Estoque</th><th>Ação</th></tr></thead><tbody>${data.items.map(p => `<tr><td>${p.name}</td><td>${p.stockCurrent || 0}</td><td><button type="button" class="btn-select-product" data-product='${JSON.stringify(p)}'>Selecionar</button></td></tr>`).join('')}</tbody></table>`
            : `<p>Nenhum produto encontrado.</p>`;
        resultsContainer.innerHTML = tableHTML;
        
        renderProductModalPagination(data);
    } catch (error) {
        resultsContainer.innerHTML = `<p style="color:red;">${error.message}</p>`;
    }
}

function renderProductModalPagination(paginationData) {
    const controlsContainer = document.getElementById('modalProductPaginationControls');
    if (!controlsContainer) return;
    controlsContainer.innerHTML = '';
    
    if(!paginationData.totalPages) {
        paginationData.totalPages = Math.ceil(paginationData.totalItems / paginationData.pageSize);
    }
    if (isNaN(paginationData.totalPages) || paginationData.totalPages <= 1) return;

    const hasPreviousPage = paginationData.page > 1;
    const hasNextPage = paginationData.page < paginationData.totalPages;

    let paginationHTML = '';
    if (hasPreviousPage) {
        paginationHTML += `<button class="pagination-btn" data-page="${paginationData.page - 1}">Anterior</button>`;
    }
    paginationHTML += `<span class="pagination-info">Página ${paginationData.page} de ${paginationData.totalPages}</span>`;
    if (hasNextPage) {
        paginationHTML += `<button class="pagination-btn" data-page="${paginationData.page + 1}">Próxima</button>`;
    }
    controlsContainer.innerHTML = paginationHTML;

    controlsContainer.querySelectorAll('.pagination-btn').forEach(button => {
        button.addEventListener('click', (event) => {
            const newPage = parseInt(event.target.dataset.page);
            fetchPaginatedProducts(newPage);
        });
    });
}


// =======================================================
// LÓGICA DA MODAL DE BUSCA DE FUNCIONÁRIOS
// =======================================================
function initializeEmployeeModal() {
    const modal = document.getElementById('employeeSearchModal');
    const openBtn = document.getElementById('openEmployeeModalBtn');
    if (!modal || !openBtn) return;
    const closeBtn = modal.querySelector('.modal-close-btn');
    const filterBtn = modal.querySelector('#modalEmployeeFilterBtn');
    const positionSelect = modal.querySelector('#modalEmployeePositionFilter');

    populatePositionFilter(positionSelect);

    openBtn.addEventListener('click', () => {
        modal.style.display = 'block';
        fetchPaginatedEmployees(1);
    });
    
    if(closeBtn) closeBtn.addEventListener('click', () => modal.style.display = 'none');
    window.addEventListener('click', (event) => { if (event.target === modal) modal.style.display = 'none'; });
    
    if(filterBtn) filterBtn.addEventListener('click', () => fetchPaginatedEmployees(1));

    modal.querySelector('#modalEmployeeResultsContainer').addEventListener('click', (event) => {
        if (event.target.classList.contains('btn-select-employee')) {
            document.getElementById('employeeId').value = event.target.dataset.id;
            document.getElementById('selectedEmployeeName').textContent = event.target.dataset.name;
            modal.style.display = 'none';
        }
    });
}

function populatePositionFilter(selectElement) {
    if (!selectElement) return;
    // positionMap é uma variável global do seu main.js
    if(typeof positionMap !== 'undefined') {
        for (const [key, value] of Object.entries(positionMap)) {
            const option = new Option(value, key);
            selectElement.appendChild(option);
        }
    }
}

async function fetchPaginatedEmployees(page = 1) {
    currentEmployeeModalPage = page;
    const resultsContainer = document.getElementById('modalEmployeeResultsContainer');
    if (!resultsContainer) return;
    resultsContainer.innerHTML = '<p>Buscando funcionários...</p>';
    try {
        const accessToken = localStorage.getItem('accessToken');
        if (!accessToken) throw new Error('Não autenticado.');
        const search = document.getElementById('modalEmployeeSearchInput')?.value;
        const position = document.getElementById('modalEmployeePositionFilter')?.value;
        const params = new URLSearchParams({ Page: page, PageSize: 10, OrderBy: 'Name' });
        if (search) params.append('Search', search);
        if (position) params.append('Positions', position);
        const url = `${API_BASE_URL}/employees/pages?${params.toString()}`;
        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error(`Falha ao buscar funcionários (Status: ${response.status})`);
        
        const paginatedData = await response.json();
        renderEmployeeModalResults(paginatedData.items);
        renderEmployeeModalPagination(paginatedData);
    } catch (error) {
        resultsContainer.innerHTML = `<p style="color:red;">${error.message}</p>`;
        document.getElementById('modalEmployeePaginationControls').innerHTML = '';
    }
}

function renderEmployeeModalResults(employees) {
    const container = document.getElementById('modalEmployeeResultsContainer');
    if (!employees || employees.length === 0) {
        container.innerHTML = '<p>Nenhum funcionário encontrado.</p>';
        return;
    }
    const tableHTML = `<table class="results-table">
        <thead><tr><th>Nome</th><th>Cargo</th><th>Ação</th></tr></thead>
        <tbody>${employees.map(e => `<tr><td>${e.name}</td><td>${getPositionName(e.positions)}</td><td><button type="button" class="btn-select-employee" data-id="${e.id}" data-name="${e.name}">Selecionar</button></td></tr>`).join('')}</tbody>
    </table>`;
    container.innerHTML = tableHTML;
}

function renderEmployeeModalPagination(paginationData) {
    const controlsContainer = document.getElementById('modalEmployeePaginationControls');
    if (!controlsContainer) return;
    controlsContainer.innerHTML = '';
    if (paginationData.totalPages <= 1) return;
    const page = paginationData.page;
    const totalPages = paginationData.totalPages;
    const prevButton = document.createElement('button');
    prevButton.textContent = 'Anterior';
    prevButton.className = 'pagination-btn';
    prevButton.disabled = page <= 1;
    prevButton.addEventListener('click', () => fetchPaginatedEmployees(page - 1));
    const pageInfo = document.createElement('span');
    pageInfo.textContent = `Página ${page} de ${totalPages}`;
    pageInfo.className = 'pagination-info';
    const nextButton = document.createElement('button');
    nextButton.textContent = 'Próxima';
    nextButton.className = 'pagination-btn';
    nextButton.disabled = page >= totalPages;
    nextButton.addEventListener('click', () => fetchPaginatedEmployees(page + 1));
    controlsContainer.appendChild(prevButton);
    controlsContainer.appendChild(pageInfo);
    controlsContainer.appendChild(nextButton);
}


// =======================================================
// LÓGICA DO HISTÓRICO DE SAÍDAS
// =======================================================
function initializeHistoryFilters() {
    const filterBtn = document.getElementById('historyFilterBtn');
    const clearBtn = document.getElementById('historyClearFilterBtn');
    if(filterBtn) filterBtn.addEventListener('click', () => fetchAndRenderHistory(1));
    if(clearBtn) clearBtn.addEventListener('click', () => {
        document.getElementById('historySearchInput').value = '';
        document.getElementById('historyCategoryFilter').value = '';
        fetchAndRenderHistory(1);
    });
}

async function fetchAndRenderHistory(page = 1) {
    currentHistoryPage = page;
    const tableBody = document.getElementById('historyTbody');
    if (!tableBody) return;
    tableBody.innerHTML = '<tr><td colspan="6" style="text-align: center;">Buscando histórico...</td></tr>';
    try {
        const accessToken = localStorage.getItem('accessToken');
        const search = document.getElementById('historySearchInput')?.value;
        const categoryId = document.getElementById('historyCategoryFilter')?.value;
        const params = new URLSearchParams({ Page: currentHistoryPage, PageSize: 10, OrderBy: 'ExitDate', Ascending: false });
        if (search) params.append('Search', search);
        if (categoryId) params.append('CategoryId', categoryId);
        const url = `${API_BASE_URL}/products-exit/paged?${params.toString()}`;
        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error(`Falha ao buscar histórico (Status: ${response.status})`);
        const paginatedData = await response.json();
        renderHistoryTable(paginatedData.items);
        renderHistoryPagination(paginatedData);
    } catch (error) {
        showErrorModal({ title: "Erro ao Listar Histórico", detail: error.message });
        tableBody.innerHTML = `<tr><td colspan="6" style="text-align: center; color: red;">${error.message}</td></tr>`;
        document.getElementById('historyPaginationControls').innerHTML = '';
    }
}

function renderHistoryTable(items) {
    const tableBody = document.getElementById('historyTbody');
    tableBody.innerHTML = '';
    if (!items || items.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="6" style="text-align: center;">Nenhum registro de saída encontrado.</td></tr>';
        return;
    }
    items.forEach(item => {
        const exitDate = new Date(item.exitDate).toLocaleDateString('pt-BR');
        const isReturnableText = item.isReturnable ? 'Sim' : 'Não';
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${item.productName || 'N/A'}</td>
            <td>${item.employeeName || 'N/A'}</td>
            <td>${item.quantity}</td>
            <td>${exitDate}</td>
            <td>${isReturnableText}</td>
            <td>${item.insertedBy || 'N/A'}</td>
        `;
        tableBody.appendChild(row);
    });
}

function renderHistoryPagination(paginationData) {
    const controlsContainer = document.getElementById('historyPaginationControls');
    if (!controlsContainer) return;
    controlsContainer.innerHTML = '';
    if (!paginationData.totalPages || paginationData.totalPages <= 1) return;
    const hasPreviousPage = paginationData.page > 1;
    const hasNextPage = paginationData.page < paginationData.totalPages;
    const prevButton = document.createElement('button');
    prevButton.textContent = 'Anterior';
    prevButton.disabled = !hasPreviousPage;
    prevButton.addEventListener('click', () => fetchAndRenderHistory(paginationData.page - 1));
    const pageInfo = document.createElement('span');
    pageInfo.textContent = `Página ${paginationData.page} de ${paginationData.totalPages}`;
    const nextButton = document.createElement('button');
    nextButton.textContent = 'Próxima';
    nextButton.disabled = !hasNextPage;
    nextButton.addEventListener('click', () => fetchAndRenderHistory(paginationData.page + 1));
    controlsContainer.appendChild(prevButton);
    controlsContainer.appendChild(pageInfo);
    controlsContainer.appendChild(nextButton);
}

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