console.log('Script js/employee.js DEFINIDO.');

// Nomes únicos para variáveis globais para evitar conflitos


// Mapa de cargos


function initializeEmployeeForm(form) {
    if (!form) return;
    form.onsubmit = (event) => {
        event.preventDefault();
        handleSaveEmployee(form);
    };
}

async function handleSaveEmployee(form) {
    const formData = new FormData(form);
    if (!formData.get('Name') || !formData.get('CPF') || !formData.get('Position')) {
        alert('Preencha Nome, CPF e Cargo.');
        return;
    }
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
            loadEmployees();
        } else {
            const errorData = await response.json();
            alert(`Erro: ${errorData.message || 'Falha ao salvar.'}`);
        }
    } catch (error) {
        alert('Erro de comunicação com o servidor.');
    }
}

async function loadEmployees() {
    const tableBody = document.querySelector('#employee-table-body');
    if (!tableBody) return;
    tableBody.innerHTML = '<tr><td colspan="5" style="text-align: center;">Buscando...</td></tr>';
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/employees`, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        const employees = await response.json();
        renderEmployeeTable(employees, tableBody);
    } catch (error) {
        tableBody.innerHTML = '<tr><td colspan="5" style="text-align: center; color: red;">Erro ao carregar.</td></tr>';
    }
}

function renderEmployeeTable(employees, tableBody) {
    tableBody.innerHTML = '';
    if (!employees || employees.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="5" style="text-align: center;">Nenhum funcionário cadastrado.</td></tr>';
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
                <td data-field="position">${getPositionName(employee.positiions)}</td>
                <td class="actions-cell" data-field="actions">
                    <button class="btn-action btn-edit" onclick='editEmployee(${employeeJsonString})'>Editar</button>
                    <button class="btn-action btn-delete" onclick="deleteEmployee('${employee.id}')">Excluir</button>
                </td>
            </tr>`;
        tableBody.insertAdjacentHTML('beforeend', rowHTML);
    });
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
        const selected = key == employee.positiions ? 'selected' : '';
        options += `<option value="${key}" ${selected}>${value}</option>`;
    }
    positionCell.innerHTML = `<select name="Positiions" class="edit-input">${options}</select>`;
    row.querySelector('[data-field="actions"]').innerHTML = `
        <button class="btn-action btn-save" onclick="saveEmployeeChanges('${employee.id}')">Salvar</button>
        <button class="btn-action btn-cancel" onclick="cancelEditEmployee('${employee.id}')">Cancelar</button>`;
};

window.saveEmployeeChanges = async (employeeId) => {
    const row = document.getElementById(`row-employee-${employeeId}`);
    if (!row) return;
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
            alert('Funcionário atualizado com sucesso!');
            loadEmployees();
        } else {
            throw new Error('Falha ao atualizar funcionário.');
        }
    } catch (error) {
        alert(error.message);
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

function initDynamicForm() {
    console.log('▶️ initDynamicForm() de employee.js foi chamada.');
    const formElement = document.querySelector('.employee-form');
    initializeEmployeeForm(formElement);
    loadEmployees();
}