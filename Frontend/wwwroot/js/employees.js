console.log('Script js/employee.js DEFINIDO.');




function initializeEmployeeForm(form) {
    if (!form) return;
    form.onsubmit = (event) => {
        event.preventDefault();
        handleSaveEmployee(form);
    };
}

async function handleSaveEmployee(form) {
    const formData = new FormData(form);
    if (!formData.get('Name') || !formData.get('CPF') || !formData.get('Positiions')) {
        alert('Preencha Nome, CPF e Cargo.');
        return;
    }

    // 1. Encontra o botão e salva o estado original
    const submitButton = form.querySelector('.submit-btn');
    const originalButtonHTML = submitButton.innerHTML;

    // 2. Desabilita o botão e mostra o spinner
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
            loadEmployees(); // Recarrega a lista
        } else {
            const errorData = await response.json();
            alert(`Erro: ${errorData.message || 'Falha ao salvar.'}`);
        }
    } catch (error) {
        alert('Erro de comunicação com o servidor.');
    } finally {
        // 3. SEMPRE restaura o botão ao final da operação
        submitButton.disabled = false;
        submitButton.innerHTML = originalButtonHTML;
    }
}

async function loadEmployees() {
    const tableBody = document.querySelector('#employee-table-body');
    if (!tableBody) return;
    tableBody.innerHTML = '<tr><td colspan="5" style="text-align: center;">Buscando...</td></tr>';

    const params = new URLSearchParams();
    const searchValue = document.getElementById('search-filter').value;
    const positionValue = document.getElementById('position-filter').value;
    const activeValue = document.getElementById('active-filter').value;

    if (searchValue) params.append('Search', searchValue);
    // Usa 'Positions' (com um 'i') para o filtro GET
    if (positionValue) params.append('Positions', positionValue);
    if (activeValue !== "") params.append('ActiveOnly', activeValue);
    params.append('Page', '1');
    params.append('PageSize', '100');

    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/employees/pages?${params.toString()}`, {
            headers: { 'Authorization': `Bearer ${accessToken}` }
        });

        if (!response.ok) throw new Error(`Erro na requisição: ${response.statusText}`);
        
        const data = await response.json();
        renderEmployeeTable(data.items || [], tableBody);
    } catch (error) {
        console.error('Erro ao carregar funcionários:', error);
        tableBody.innerHTML = '<tr><td colspan="5" style="text-align: center; color: red;">Erro ao carregar. Tente novamente.</td></tr>';
    }
}

function renderEmployeeTable(employees, tableBody) {
    tableBody.innerHTML = '';
    if (!employees || employees.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="5" style="text-align: center;">Nenhum funcionário encontrado.</td></tr>';
        return;
    }
    employees.forEach(employee => {
        const imageUrl = employee.imageUrl || 'https://via.placeholder.com/60';
        const employeeJsonString = JSON.stringify(employee).replace(/'/g, "&apos;");
        const rowHTML = `
            <tr id="row-employee-${employee.id}">
                <td data-field="image"><img src="${imageUrl}" alt="${employee.name}" class="product-table-img"></td>
                <td data-field="name">${employee.name}</td>
                <td data-field="cpf">${employee.cpf}</td>
                <td data-field="position">${getPositionName(employee.positions)}</td>
                <td class="actions-cell" data-field="actions">
                    <button class="btn-action btn-edit" onclick='editEmployee(${employeeJsonString})'>Editar</button>
                    <button class="btn-action btn-delete" onclick="deleteEmployee('${employee.id}')">Excluir</button>
                </td>
            </tr>`;
        tableBody.insertAdjacentHTML('beforeend', rowHTML);
    });
}

function populatePositionFilter() {
    const select = document.getElementById('position-filter');
    if (!select) return;
    select.innerHTML = '<option value="">Todos os Cargos</option>';
    for (const [key, value] of Object.entries(positionMap)) {
        select.innerHTML += `<option value="${key}">${value}</option>`;
    }
}

function initializeFilters() {
    const filterForm = document.getElementById('filter-form');
    const clearButton = document.getElementById('clear-filters-btn');

    if (filterForm) {
        filterForm.onsubmit = (event) => {
            event.preventDefault();
            loadEmployees();
        };
    }

    if (clearButton) {
        clearButton.onclick = () => {
            filterForm.reset();
            loadEmployees();
        };
    }
}

window.deleteEmployee = async (employeeId) => {
    if (!confirm('Tem certeza que deseja excluir este funcionário?')) return;
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/employees/${employeeId}`, { method: 'DELETE', headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (response.ok) {
            alert('Funcionário excluído!');
            loadEmployees();
        } else {
            throw new Error('Falha ao excluir.');
        }
    } catch (error) {
        alert(error.message);
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
    // Usa 'Positiions' (com dois 'i's') para o POST/PUT
    positionCell.innerHTML = `<select name="Positiions" class="edit-input">${options}</select>`;
    row.querySelector('[data-field="actions"]').innerHTML = `
        <button class="btn-action btn-save" onclick="saveEmployeeChanges('${employee.id}')">Salvar</button>
        <button class="btn-action btn-cancel" onclick="cancelEditEmployee('${employee.id}')">Cancelar</button>`;
};

window.saveEmployeeChanges = async (employeeId) => {
    const row = document.getElementById(`row-employee-${employeeId}`);
    if (!row) return;

    // 1. Encontra o botão de salvar na linha
    const saveButton = row.querySelector('.btn-save');

    // 2. Desabilita o botão e mostra o spinner
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
        if (response.ok) {
            // Não precisa de alerta, a atualização da tabela é o feedback
            loadEmployees();
        } else {
            const errorData = await response.json();
            throw new Error(errorData.message || 'Falha ao atualizar funcionário.');
        }
    } catch (error) {
        alert(error.message);
        // 3. Em caso de erro, a linha é restaurada, removendo o botão de loading
        cancelEditEmployee(employeeId);
    }
    // O bloco finally não é necessário aqui, pois a linha sempre será
    // redesenhada pelo loadEmployees() ou cancelEditEmployee().
};

window.cancelEditEmployee = (employeeId) => {
    const row = document.getElementById(`row-employee-${employeeId}`);
    if (row && originalRowHTML_Employee[employeeId]) {
        row.innerHTML = originalRowHTML_Employee[employeeId];
        delete originalRowHTML_Employee[employeeId];
    }
};

// Função principal de inicialização
function initDynamicForm() {
    console.log('▶️ initDynamicForm() de employee.js foi chamada.');
    const formElement = document.querySelector('.employee-form');
    initializeEmployeeForm(formElement);
    
    populatePositionFilter();
    initializeFilters();
    
    loadEmployees(); // Carga inicial
}