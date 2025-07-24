console.log('Script js/employee.js DEFINIDO.');

// Utiliza as variáveis globais de main.js


// =======================================================
// INICIALIZAÇÃO DA PÁGINA
// =======================================================
function initDynamicForm() {
    console.log('▶️ initDynamicForm() de employee.js foi chamada.');
    initializeEmployeeForm(document.querySelector('.employee-form'));
    initializeFilters();
    populatePositionFilter();
    loadEmployees(1);
}

function initializeFilters() {
    document.getElementById('filter-btn')?.addEventListener('click', () => loadEmployees(1));
    document.getElementById('clear-filters-btn')?.addEventListener('click', () => {
        document.getElementById('search-filter').value = '';
        document.getElementById('position-filter').value = '';
        loadEmployees(1);
    });
}

function populatePositionFilter() {
    const select = document.getElementById('position-filter');
    if (!select || typeof positionMap === 'undefined') return;
    select.innerHTML = '<option value="">Todos os Cargos</option>';
    for (const [key, value] of Object.entries(positionMap)) {
        const option = new Option(value, key);
        select.appendChild(option);
    }
}

// =======================================================
// FORMULÁRIO DE CADASTRO
// =======================================================
function initializeEmployeeForm(form) {
    if (!form) return;
    form.onsubmit = (event) => {
        event.preventDefault();
        handleSaveEmployee(form);
    };
}

async function handleSaveEmployee(form) {
    const formData = new FormData(form);
    const submitButton = form.querySelector('.submit-btn');
    const originalButtonHTML = submitButton.innerHTML;
    submitButton.disabled = true;
    submitButton.innerHTML = `<span class="loading-spinner"></span> Salvando...`;

    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/employees`, {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${accessToken}` },
            body: formData,
        });
        if (response.ok) {
            alert('Funcionário salvo com sucesso!');
            form.reset();
            loadEmployees(1);
        } else {
            const errorData = await response.json();
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: error.message });
    } finally {
        submitButton.disabled = false;
        submitButton.innerHTML = originalButtonHTML;
    }
}

// =======================================================
// LÓGICA DA TABELA (PAGINAÇÃO NO SERVIDOR)
// =======================================================
async function loadEmployees(page = 1) {
    currentEmployeePage = page;
    const tableBody = document.querySelector('#employee-table-body');
    if (!tableBody) return;
    tableBody.innerHTML = '<tr><td colspan="5" style="text-align: center;">Buscando...</td></tr>';
    
    try {
        const accessToken = localStorage.getItem('accessToken');
        const search = document.getElementById('search-filter')?.value;
        const position = document.getElementById('position-filter')?.value;

        const params = new URLSearchParams({ Page: currentEmployeePage, PageSize: 10, OrderBy: 'Name' });
        if (search) params.append('Search', search);
        if (position) params.append('Positions', position);

        const url = `${API_BASE_URL}/employees/pages?${params.toString()}`;
        const response = await fetch(url, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error(`Falha ao buscar funcionários (Status: ${response.status})`);
        
        const paginatedData = await response.json();
        renderEmployeeTable(paginatedData.items);
        renderPagination(paginatedData);
    } catch (error) {
        showErrorModal({ title: "Erro ao Listar", detail: error.message });
        tableBody.innerHTML = `<tr><td colspan="5" style="text-align: center; color: red;">${error.message}</td></tr>`;
        const paginationControls = document.getElementById('pagination-controls');
        if (paginationControls) paginationControls.innerHTML = '';
    }
}

function renderEmployeeTable(employees) {
    const tableBody = document.querySelector('#employee-table-body');
    tableBody.innerHTML = '';
    if (!employees || employees.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="5" style="text-align: center;">Nenhum funcionário encontrado.</td></tr>';
        return;
    }
    employees.forEach(employee => {
        const imageUrl = employee.imageUrl || 'https://via.placeholder.com/60';
        const employeeJsonString = JSON.stringify(employee).replace(/'/g, "&apos;");
        const rowHTML = `<tr id="row-employee-${employee.id}">
                <td><img src="${imageUrl}" alt="${employee.name}" class="product-table-img"></td>
                <td data-field="name">${employee.name}</td>
                <td data-field="cpf">${employee.cpf}</td>
                <td data-field="position">${getPositionName(employee.positions)}</td>
                <td class="actions-cell">
                    <button class="btn-action btn-edit" onclick='editEmployee(${employeeJsonString})'>Editar</button>
                    <button class="btn-action btn-delete" onclick="deleteEmployee('${employee.id}')">Excluir</button>
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
    prevButton.onclick = () => loadEmployees(page - 1);
    
    const pageInfo = document.createElement('span');
    pageInfo.textContent = `Página ${page} de ${totalPages}`;
    pageInfo.className = 'pagination-info';
    
    const nextButton = document.createElement('button');
    nextButton.textContent = 'Próxima';
    nextButton.className = 'pagination-btn';
    nextButton.disabled = page >= totalPages;
    nextButton.onclick = () => loadEmployees(page + 1);
    
    controlsContainer.appendChild(prevButton);
    controlsContainer.appendChild(pageInfo);
    controlsContainer.appendChild(nextButton);
}

window.deleteEmployee = async (employeeId) => {
    if (!confirm('Tem certeza que deseja excluir este funcionário?')) return;
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/employees/${employeeId}`, { method: 'DELETE', headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (response.ok) {
            alert('Funcionário excluído com sucesso!');
            loadEmployees(currentEmployeePage);
        } else {
            const errorData = await response.json().catch(() => ({ title: "Erro ao Excluir" }));
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: error.message });
    }
};

window.editEmployee = (employee) => {
    const row = document.getElementById(`row-employee-${employee.id}`);
    if (!row) return;
    originalRowHTML_Employee[employee.id] = row.innerHTML;
    row.querySelector('[data-field="name"]').innerHTML = `<input type="text" name="Name" class="edit-input" value="${employee.name}">`;
    row.querySelector('[data-field="cpf"]').innerHTML = `<input type="text" name="CPF" class="edit-input" value="${employee.cpf}">`;
    const positionCell = row.querySelector('[data-field="position"]');
    let options = '';
    for (const [key, value] of Object.entries(positionMap)) {
        const selected = key == employee.positions ? 'selected' : '';
        options += `<option value="${key}" ${selected}>${value}</option>`;
    }
    positionCell.innerHTML = `<select name="Positiions" class="edit-input">${options}</select>`;
    row.querySelector('.actions-cell').innerHTML = `
        <button class="btn-action btn-save" onclick="saveEmployeeChanges('${employee.id}')">Salvar</button>
        <button class="btn-action btn-cancel" onclick="cancelEditEmployee('${employee.id}')">Cancelar</button>`;
};

window.saveEmployeeChanges = async (employeeId) => {
    const row = document.getElementById(`row-employee-${employeeId}`);
    if (!row) return;
    const saveButton = row.querySelector('.btn-save');
    saveButton.disabled = true;
    saveButton.innerHTML = `<span class="loading-spinner"></span>`;

    const formData = new FormData();
    formData.append('Id', employeeId);
    formData.append('Name', row.querySelector('[name="Name"]').value);
    formData.append('CPF', row.querySelector('[name="CPF"]').value);
    formData.append('Positiions', row.querySelector('[name="Positiions"]').value);

    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/employees`, {
            method: 'PUT',
            headers: { 'Authorization': `Bearer ${accessToken}` },
            body: formData
        });
        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || 'Falha ao atualizar funcionário.');
        }
        loadEmployees(currentEmployeePage);
    } catch (error) {
        showErrorModal({ title: "Erro ao Salvar", detail: error.message });
        cancelEditEmployee(employeeId);
    }
};

window.cancelEditEmployee = (employeeId) => {
    const row = document.getElementById(`row-employee-${employeeId}`);
    if (row && originalRowHTML_Employee[employeeId]) {
        row.innerHTML = originalRowHTML_Employee[employeeId];
        delete originalRowHTML_Employee[employeeId];
    }
};