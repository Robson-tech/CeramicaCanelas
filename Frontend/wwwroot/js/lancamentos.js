console.log('Script js/lancamento.js DEFINIDO.');



// =======================================================
// INICIALIZAÇÃO
// =======================================================
function initDynamicForm() {
    console.log('▶️ initDynamicForm() de lancamento.js foi chamada.');
    initializeLaunchForm();
    initializeLaunchCategoryModal();
    initializeCustomerModal();
    initializeHistoryFilters();
    fetchAndRenderHistory(1);
}

// =======================================================
// LÓGICA DO FORMULÁRIO DE CADASTRO
// =======================================================
function initializeLaunchForm() {
    const typeSelection = document.getElementById('type-selection-group');
    const launchForm = document.getElementById('launchForm');
    const statusSelect = document.getElementById('status');
    populateEnumSelects();
    typeSelection.addEventListener('change', (event) => updateFormVisibility(event.target.value));
    statusSelect.addEventListener('change', (event) => {
        document.getElementById('group-dueDate').style.display = (event.target.value === '0') ? 'block' : 'none';
    });
    launchForm.addEventListener('submit', handleLaunchSubmit);
}

function updateFormVisibility(type) {
    const launchForm = document.getElementById('launchForm');
    const categoryGroup = document.getElementById('group-categoryId');
    const customerGroup = document.getElementById('group-customerId');
    launchForm.style.display = 'block';
    categoryGroup.style.display = (type === '2') ? 'block' : 'none';
    customerGroup.style.display = (type === '1') ? 'block' : 'none';
}

function populateEnumSelects() {
    const paymentSelect = document.getElementById('paymentMethod');
    const statusSelect = document.getElementById('status');
    for (const [key, value] of Object.entries(paymentMethodMap)) {
        paymentSelect.appendChild(new Option(value, key));
    }
    for (const [key, value] of Object.entries(statusMap)) {
        const option = new Option(value, key);
        // Define 'Pago' (valor 1) como padrão
        if (key === '1') {
            option.selected = true;
        }
        statusSelect.appendChild(option);
    }
}

async function handleLaunchSubmit(event) {
    event.preventDefault();
    const form = event.target;
    const formData = new FormData(form);
    const selectedType = document.querySelector('input[name="Type"]:checked');
    if (!selectedType) {
        showErrorModal({ title: "Validação Falhou", detail: "Por favor, selecione se é uma Entrada ou Saída."});
        return;
    }
    formData.append('Type', selectedType.value);
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/financial/launch`, {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${accessToken}` },
            body: formData
        });
        if (response.ok) {
            alert('Lançamento salvo com sucesso!');
            form.reset();
            populateEnumSelects(); // Repopula o select para restaurar o padrão
            document.getElementById('selectedCategoryName').textContent = 'Nenhuma categoria selecionada';
            document.getElementById('selectedCustomerName').textContent = 'Nenhum cliente selecionado';
            selectedType.checked = false;
            form.style.display = 'none';
            fetchAndRenderHistory(1);
        } else {
            const errorData = await response.json();
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: error.message });
    }
}

// =======================================================
// LÓGICA DAS MODAIS
// =======================================================
function initializeLaunchCategoryModal() {
    const modal = document.getElementById('categorySearchModal');
    const openBtn = document.getElementById('openCategoryModalBtn');
    if (!modal || !openBtn) return;
    const closeBtn = modal.querySelector('.modal-close-btn');
    const filterBtn = modal.querySelector('#modalCategoryFilterBtn');
    openBtn.addEventListener('click', (e) => {
        e.preventDefault();
        modal.style.display = 'block';
        fetchAndRenderLaunchCategoriesInModal(1);
    });
    if(closeBtn) closeBtn.addEventListener('click', () => modal.style.display = 'none');
    if(filterBtn) filterBtn.addEventListener('click', () => fetchAndRenderLaunchCategoriesInModal(1));
    modal.querySelector('#modalCategoryResultsContainer').addEventListener('click', (event) => {
        if (event.target.classList.contains('select-category-btn')) {
            document.getElementById('selectedCategoryName').textContent = event.target.dataset.name;
            document.getElementById('categoryId').value = event.target.dataset.id;
            modal.style.display = 'none';
        }
    });
}

function initializeCustomerModal() {
    const modal = document.getElementById('customerSearchModal');
    const openBtn = document.getElementById('openCustomerModalBtn');
    if (!modal || !openBtn) return;
    const closeBtn = modal.querySelector('.modal-close-btn');
    const filterBtn = modal.querySelector('#modalCustomerFilterBtn');
    openBtn.addEventListener('click', (e) => {
        e.preventDefault();
        modal.style.display = 'block';
        fetchAndRenderCustomersInModal(1);
    });
    if(closeBtn) closeBtn.addEventListener('click', () => modal.style.display = 'none');
    if(filterBtn) filterBtn.addEventListener('click', () => fetchAndRenderCustomersInModal(1));
    modal.querySelector('#modalCustomerResultsContainer').addEventListener('click', (event) => {
        if (event.target.classList.contains('select-customer-btn')) {
            document.getElementById('selectedCustomerName').textContent = event.target.dataset.name;
            document.getElementById('customerId').value = event.target.dataset.id;
            modal.style.display = 'none';
        }
    });
}

async function fetchAndRenderLaunchCategoriesInModal(page = 1) {
    currentCategoryModalPage = page;
    const resultsContainer = document.getElementById('modalCategoryResultsContainer');
    if (!resultsContainer) return;
    resultsContainer.innerHTML = '<p>Buscando categorias...</p>';
    try {
        const accessToken = localStorage.getItem('accessToken');
        const search = document.getElementById('modalCategorySearchInput').value;
        const params = new URLSearchParams({ Page: page, PageSize: 10, OrderBy: 'Name' });
        if (search) params.append('Search', search);
        const url = `${API_BASE_URL}/financial/launch-categories/paged?${params.toString()}`;
        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error(`Falha na requisição: ${response.status}`);
        const paginatedData = await response.json();
        renderLaunchCategoryModalResults(paginatedData.items, resultsContainer);
        renderLaunchCategoryModalPagination(paginatedData);
    } catch (error) {
        resultsContainer.innerHTML = `<p style="color:red;">${error.message}</p>`;
        document.getElementById('modalCategoryPaginationControls').innerHTML = '';
    }
}

function renderLaunchCategoryModalResults(categories, container) {
    if (!categories || categories.length === 0) {
        container.innerHTML = '<p>Nenhuma categoria encontrada.</p>';
        return;
    }
    const table = document.createElement('table');
    table.className = 'results-table';
    table.innerHTML = `<thead><tr><th>Nome</th><th>Ação</th></tr></thead><tbody>
        ${categories.map(cat => `<tr><td>${cat.name}</td><td><button type="button" class="select-category-btn" data-id="${cat.id}" data-name="${cat.name}">Selecionar</button></td></tr>`).join('')}
    </tbody>`;
    container.innerHTML = '';
    container.appendChild(table);
}

function renderLaunchCategoryModalPagination(paginationData) {
    const controlsContainer = document.getElementById('modalCategoryPaginationControls');
    if (!controlsContainer) return;
    controlsContainer.innerHTML = '';
    if (paginationData.totalPages <= 1) return;
    const { page, totalPages } = paginationData;
    const prevButton = document.createElement('button');
    prevButton.textContent = 'Anterior';
    prevButton.className = 'pagination-btn';
    prevButton.disabled = page <= 1;
    prevButton.onclick = () => fetchAndRenderLaunchCategoriesInModal(page - 1);
    const pageInfo = document.createElement('span');
    pageInfo.textContent = `Página ${page} de ${totalPages}`;
    pageInfo.className = 'pagination-info';
    const nextButton = document.createElement('button');
    nextButton.textContent = 'Próxima';
    nextButton.className = 'pagination-btn';
    nextButton.disabled = page >= totalPages;
    nextButton.onclick = () => fetchAndRenderLaunchCategoriesInModal(page + 1);
    controlsContainer.appendChild(prevButton);
    controlsContainer.appendChild(pageInfo);
    controlsContainer.appendChild(nextButton);
}

async function fetchAndRenderCustomersInModal(page = 1) {
    currentCustomerModalPage = page;
    const resultsContainer = document.getElementById('modalCustomerResultsContainer');
    if (!resultsContainer) return;
    resultsContainer.innerHTML = '<p>Buscando clientes...</p>';
    try {
        const accessToken = localStorage.getItem('accessToken');
        const search = document.getElementById('modalCustomerSearchInput').value;
        const params = new URLSearchParams({ Page: page, PageSize: 10, OrderBy: 'Name' });
        if (search) params.append('Search', search);
        const url = `${API_BASE_URL}/financial/customer/paged?${params.toString()}`;
        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error(`Falha na requisição: ${response.status}`);
        const paginatedData = await response.json();
        renderCustomerModalResults(paginatedData.items, resultsContainer);
        renderCustomerModalPagination(paginatedData);
    } catch (error) {
        resultsContainer.innerHTML = `<p style="color:red;">${error.message}</p>`;
        document.getElementById('modalCustomerPaginationControls').innerHTML = '';
    }
}

function renderCustomerModalResults(customers, container) {
    if (!customers || customers.length === 0) {
        container.innerHTML = '<p>Nenhum cliente encontrado.</p>';
        return;
    }
    const table = document.createElement('table');
    table.className = 'results-table';
    table.innerHTML = `<thead><tr><th>Nome</th><th>Documento</th><th>Ação</th></tr></thead><tbody>
        ${customers.map(c => `<tr><td>${c.name}</td><td>${c.document || 'N/A'}</td><td><button type="button" class="select-customer-btn" data-id="${c.id}" data-name="${c.name}">Selecionar</button></td></tr>`).join('')}
    </tbody>`;
    container.innerHTML = '';
    container.appendChild(table);
}

function renderCustomerModalPagination(paginationData) {
    const controlsContainer = document.getElementById('modalCustomerPaginationControls');
    if (!controlsContainer) return;
    controlsContainer.innerHTML = '';
    if (paginationData.totalPages <= 1) return;
    const { page, totalPages } = paginationData;
    const prevButton = document.createElement('button');
    prevButton.textContent = 'Anterior';
    prevButton.className = 'pagination-btn';
    prevButton.disabled = page <= 1;
    prevButton.onclick = () => fetchAndRenderCustomersInModal(page - 1);
    const pageInfo = document.createElement('span');
    pageInfo.textContent = `Página ${page} de ${totalPages}`;
    pageInfo.className = 'pagination-info';
    const nextButton = document.createElement('button');
    nextButton.textContent = 'Próxima';
    nextButton.className = 'pagination-btn';
    nextButton.disabled = page >= totalPages;
    nextButton.onclick = () => fetchAndRenderCustomersInModal(page + 1);
    controlsContainer.appendChild(prevButton);
    controlsContainer.appendChild(pageInfo);
    controlsContainer.appendChild(nextButton);
}

// =======================================================
// LÓGICA DA TABELA DE HISTÓRICO
// =======================================================
function initializeHistoryFilters() {
    const filterBtn = document.getElementById('historyFilterBtn');
    const clearBtn = document.getElementById('historyClearBtn');
    const typeSelect = document.getElementById('historyType');
    const statusSelect = document.getElementById('historyStatus');
    
    typeSelect.innerHTML = '<option value="">Todos os Tipos</option>';
    statusSelect.innerHTML = '<option value="">Todos os Status</option>';
    for (const [key, value] of Object.entries(launchTypeMap)) {
        typeSelect.appendChild(new Option(value, key));
    }
    for (const [key, value] of Object.entries(statusMap)) {
        statusSelect.appendChild(new Option(value, key));
    }

    if(filterBtn) filterBtn.onclick = () => fetchAndRenderHistory(1);
    if(clearBtn) clearBtn.onclick = () => {
        document.getElementById('historySearch').value = '';
        typeSelect.value = '';
        statusSelect.value = '';
        fetchAndRenderHistory(1);
    };
}

async function fetchAndRenderHistory(page = 1) {
    currentHistoryPage = page;
    const tableBody = document.querySelector('#launch-history-body');
    if (!tableBody) return;
    tableBody.innerHTML = '<tr><td colspan="6" style="text-align: center;">Buscando...</td></tr>';
    try {
        const accessToken = localStorage.getItem('accessToken');
        const params = new URLSearchParams({ Page: page, PageSize: 10, OrderBy: 'LaunchDate', Ascending: false });
        const search = document.getElementById('historySearch')?.value;
        const type = document.getElementById('historyType')?.value;
        const status = document.getElementById('historyStatus')?.value;
        if(search) params.append('Search', search);
        if(type) params.append('Type', type);
        if(status) params.append('Status', status);
        const url = `${API_BASE_URL}/financial/launch/paged?${params.toString()}`;
        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error(`Falha ao buscar lançamentos (Status: ${response.status})`);
        const paginatedData = await response.json();
        historyItemsCache = paginatedData.items;
        renderHistoryTable(paginatedData.items, tableBody);
        renderHistoryPagination(paginatedData);
    } catch (error) {
        showErrorModal({ title: "Erro ao Listar", detail: error.message });
        tableBody.innerHTML = `<tr><td colspan="6" style="text-align: center; color: red;">${error.message}</td></tr>`;
    }
}

function renderHistoryTable(items, tableBody) {
    tableBody.innerHTML = '';
    if (!items || items.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="6" style="text-align: center;">Nenhum lançamento encontrado.</td></tr>';
        return;
    }
    items.forEach(item => {
        const itemJsonString = JSON.stringify(item).replace(/'/g, "&apos;");
        const formattedDate = new Date(item.launchDate).toLocaleDateString('pt-BR');
        const formattedAmount = (item.amount || 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
        const typeText = launchTypeMap[item.type] || 'N/A';
        const statusText = statusMap[item.status] || 'N/A';
        const amountClass = item.type === 1 ? 'income' : 'expense';
        const rowHTML = `
            <tr id="row-launch-${item.id}">
                <td data-field="description">${item.description}</td>
                <td data-field="amount" class="${amountClass}">${formattedAmount}</td>
                <td data-field="launchDate">${formattedDate}</td>
                <td data-field="type">${typeText}</td>
                <td data-field="status">${statusText}</td>
                <td class="actions-cell" data-field="actions">
                    <button class="btn-action btn-edit" onclick='editLaunch(${itemJsonString})'>Editar</button>
                    <button class="btn-action btn-delete" onclick="deleteLaunch('${item.id}')">Excluir</button>
                </td>
            </tr>`;
        tableBody.insertAdjacentHTML('beforeend', rowHTML);
    });
}

function renderHistoryPagination(paginationData) {
    const controlsContainer = document.getElementById('pagination-controls');
    if (!controlsContainer) return;
    controlsContainer.innerHTML = '';
    if (!paginationData || paginationData.totalPages <= 1) return;
    const { page, totalPages } = paginationData;
    const prevButton = document.createElement('button');
    prevButton.textContent = 'Anterior';
    prevButton.className = 'pagination-btn';
    prevButton.disabled = page <= 1;
    prevButton.onclick = () => fetchAndRenderHistory(page - 1);
    const pageInfo = document.createElement('span');
    pageInfo.textContent = `Página ${page} de ${totalPages}`;
    pageInfo.className = 'pagination-info';
    const nextButton = document.createElement('button');
    nextButton.textContent = 'Próxima';
    nextButton.className = 'pagination-btn';
    nextButton.disabled = page >= totalPages;
    nextButton.onclick = () => fetchAndRenderHistory(page + 1);
    controlsContainer.appendChild(prevButton);
    controlsContainer.appendChild(pageInfo);
    controlsContainer.appendChild(nextButton);
}

window.deleteLaunch = async (launchId) => {
    if (!confirm('Tem certeza que deseja excluir este lançamento?')) return;
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/financial/launch/${launchId}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${accessToken}` }
        });
        if (response.ok) {
            alert('Lançamento excluído com sucesso!');
            fetchAndRenderHistory(currentHistoryPage);
        } else {
            const errorData = await response.json().catch(() => ({ title: "Erro ao Excluir" }));
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: error.message });
    }
};

window.editLaunch = (item) => {
    const row = document.getElementById(`row-launch-${item.id}`);
    if (!row) return;
    originalRowHTML_Launch[item.id] = row.innerHTML;
    
    row.querySelector('[data-field="description"]').innerHTML = `<textarea name="Description" class="edit-input">${item.description}</textarea>`;
    row.querySelector('[data-field="amount"]').innerHTML = `<input type="number" name="Amount" class="edit-input" value="${item.amount}" step="0.01">`;
    
    const isoDate = new Date(item.launchDate).toISOString().split('T')[0];
    row.querySelector('[data-field="launchDate"]').innerHTML = `<input type="date" name="LaunchDate" class="edit-input" value="${isoDate}">`;

    let statusOptions = '';
    for(const [key, value] of Object.entries(statusMap)) {
        const selected = key == item.status ? 'selected' : '';
        statusOptions += `<option value="${key}" ${selected}>${value}</option>`;
    }
    row.querySelector('[data-field="status"]').innerHTML = `<select name="Status" class="edit-input">${statusOptions}</select>`;

    row.querySelector('[data-field="actions"]').innerHTML = `
        <button class="btn-action btn-save" onclick="saveLaunchChanges('${item.id}')">Salvar</button>
        <button class="btn-action btn-cancel" onclick="cancelLaunchEdit('${item.id}')">Cancelar</button>
    `;
};

window.saveLaunchChanges = async (launchId) => {
    const row = document.getElementById(`row-launch-${launchId}`);
    if (!row) return;
    
    const formData = new FormData();
    formData.append('Id', launchId);
    formData.append('Description', row.querySelector('[name="Description"]').value);
    formData.append('Amount', row.querySelector('[name="Amount"]').value);
    formData.append('LaunchDate', new Date(row.querySelector('[name="LaunchDate"]').value).toISOString());
    formData.append('Status', row.querySelector('[name="Status"]').value);

    const originalItem = historyItemsCache.find(i => i.id === launchId);
    if(originalItem) {
        formData.append('Type', originalItem.type);
        if(originalItem.categoryId) formData.append('CategoryId', originalItem.categoryId);
        if(originalItem.customerId) formData.append('CustomerId', originalItem.customerId);
        if(originalItem.paymentMethod) formData.append('PaymentMethod', originalItem.paymentMethod);
    }

    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/financial/launch`, {
            method: 'PUT',
            headers: { 'Authorization': `Bearer ${accessToken}` },
            body: formData
        });
        if (response.ok) {
            alert('Lançamento atualizado com sucesso!');
            fetchAndRenderHistory(currentHistoryPage);
        } else {
            const errorData = await response.json().catch(() => ({ title: "Erro ao Salvar" }));
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: error.message });
        cancelLaunchEdit(launchId);
    }
};

window.cancelLaunchEdit = (launchId) => {
    const row = document.getElementById(`row-launch-${launchId}`);
    if (row && originalRowHTML_Launch[launchId]) {
        row.innerHTML = originalRowHTML_Launch[launchId];
        delete originalRowHTML_Launch[launchId];
    }
};