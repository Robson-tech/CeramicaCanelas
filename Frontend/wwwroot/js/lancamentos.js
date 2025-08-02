console.log('Script js/lancamento.js DEFINIDO.');


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
    for (const [key, value] of Object.entries(paymentMethodMap)) paymentSelect.appendChild(new Option(value, key));
    for (const [key, value] of Object.entries(statusMap)) statusSelect.appendChild(new Option(value, key));
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
function initializeLaunchCategoryModal() { /* ...código da resposta anterior... */ }
function initializeCustomerModal() { /* ...código da resposta anterior... */ }
async function fetchAndRenderLaunchCategoriesInModal(page = 1) { /* ...código da resposta anterior... */ }
function renderLaunchCategoryModalResults(categories, container) { /* ...código da resposta anterior... */ }
function renderLaunchCategoryModalPagination(paginationData) { /* ...código da resposta anterior... */ }
async function fetchAndRenderCustomersInModal(page = 1) { /* ...código da resposta anterior... */ }
function renderCustomerModalResults(customers, container) { /* ...código da resposta anterior... */ }
function renderCustomerModalPagination(paginationData) { /* ...código da resposta anterior... */ }

// =======================================================
// LÓGICA DA TABELA DE HISTÓRICO (COM CRUD)
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