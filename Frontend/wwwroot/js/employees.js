console.log('✅ SCRIPT: js/employee.js foi DEFINIDO.');

const API_URL = 'http://localhost:5087/api/employees';

// Mapa para converter o número da posição para o nome do cargo
const positionMap = {
    0: 'Enfornador', 1: 'Desenfornador', 2: 'Soldador', 3: 'Marombeiro',
    4: 'Operador de Pá Carregadeira', 5: 'Motorista', 6: 'Queimador',
    7: 'Conferente', 8: 'Caixa', 9: 'Auxiliar Administrativo',
    10: 'Auxiliar de Limpeza', 11: 'Dono', 12: 'Gerente', 13: 'Auxiliar de Estoque'
};

const getPositionName = (positionId) => positionMap[positionId] || 'Desconhecido';

// Objeto para armazenar o estado original da linha antes da edição
const originalRowHTML = {};


// =================================================================
// FUNÇÕES DA TABELA (GET, RENDER, EDIT, DELETE)
// =================================================================

async function loadEmployees() {
    console.log('Buscando lista de funcionários para a tabela...');
    const accessToken = localStorage.getItem('accessToken');
    if (!accessToken) {
        console.warn('Token de acesso não encontrado. A tabela não será carregada.');
        return;
    }
    try {
        const response = await fetch(API_URL, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error('Falha ao buscar funcionários.');
        const employees = await response.json();
        renderEmployeeTable(employees);
    } catch (error) {
        console.error('Erro ao carregar funcionários para a tabela:', error);
    }
}

function renderEmployeeTable(employees) {
    const tableBody = document.getElementById('employee-table-body');
    if (!tableBody) return;
    tableBody.innerHTML = '';
    employees.forEach(employee => {
        const positionValue = employee.positiions;
        const imageUrl = employee.imageUrl || 'https://via.placeholder.com/60';
        const row = `
            <tr id="row-${employee.id}" data-position="${positionValue}">
                <td data-field="image"><img src="${imageUrl}" alt="${employee.name}" class="employee-photo"></td>
                <td data-field="name">${employee.name}</td>
                <td data-field="cpf">${employee.cpf}</td>
                <td data-field="position">${getPositionName(positionValue)}</td>
                <td data-field="actions">
                    <button class="btn-edit" onclick="editEmployee('${employee.id}')">Editar</button>
                    <button class="btn-delete" onclick="deleteEmployee('${employee.id}')">Excluir</button>
                </td>
            </tr>
        `;
        tableBody.insertAdjacentHTML('beforeend', row);
    });
}

window.deleteEmployee = async (employeeId) => {
    if (!confirm('Tem certeza que deseja excluir este funcionário?')) return;
    const accessToken = localStorage.getItem('accessToken');
    if (!accessToken) { alert('Autenticação necessária.'); return; }
    try {
        const response = await fetch(`${API_URL}/${employeeId}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${accessToken}` }
        });
        if (response.ok) {
            alert('Funcionário excluído com sucesso!');
            loadEmployees();
        } else {
            throw new Error('Falha ao excluir funcionário.');
        }
    } catch (error) {
        console.error('Erro ao excluir:', error);
        alert(error.message);
    }
};

window.editEmployee = (employeeId) => {
    document.querySelectorAll('.btn-edit').forEach(btn => btn.disabled = true);
    const row = document.getElementById(`row-${employeeId}`);
    if (!row) return;
    originalRowHTML[employeeId] = row.innerHTML;
    
    const imageCell = row.querySelector('[data-field="image"]');
    const nameCell = row.querySelector('[data-field="name"]');
    const cpfCell = row.querySelector('[data-field="cpf"]');
    const positionCell = row.querySelector('[data-field="position"]');
    const actionsCell = row.querySelector('[data-field="actions"]');
    
    const currentName = nameCell.innerText;
    const currentCpf = cpfCell.innerText;
    const currentPositionValue = row.getAttribute('data-position');
    const currentImageHTML = imageCell.innerHTML;

    imageCell.innerHTML = `${currentImageHTML}<br><label style="font-size: 12px; margin-top: 5px; display: block;">Trocar Imagem:<input type="file" class="edit-file" accept="image/*"></label>`;
    
    nameCell.innerHTML = `<input type="text" class="edit-input" value="${currentName}">`;
    cpfCell.innerHTML = `<input type="text" class="edit-input" value="${currentCpf}">`;
    let positionOptions = '';
    for (const [key, value] of Object.entries(positionMap)) {
        const isSelected = key === currentPositionValue ? 'selected' : '';
        positionOptions += `<option value="${key}" ${isSelected}>${value}</option>`;
    }
    positionCell.innerHTML = `<select class="edit-select">${positionOptions}</select>`;
    
    actionsCell.innerHTML = `
        <button class="btn-save" onclick="saveEmployeeChanges('${employeeId}')">Salvar</button>
        <button class="btn-cancel" onclick="cancelEdit('${employeeId}')">Cancelar</button>
    `;
};

window.saveEmployeeChanges = async (employeeId) => {
    const row = document.getElementById(`row-${employeeId}`);
    if (!row) return;
    const accessToken = localStorage.getItem('accessToken');
    if (!accessToken) {
        alert('Autenticação necessária.');
        return;
    }

    const formData = new FormData();
    const nameValue = row.querySelector('[data-field="name"] input').value;
    const cpfValue = row.querySelector('[data-field="cpf"] input').value;
    const positionValue = row.querySelector('[data-field="position"] select').value;
    const imageFile = row.querySelector('.edit-file').files[0];

    formData.append('Id', employeeId);
    formData.append('Name', nameValue);
    formData.append('CPF', cpfValue);
    formData.append('Positiions', positionValue);
    
    if (imageFile) {
        formData.append('Imagem', imageFile);
    }

    try {
        const response = await fetch(API_URL, {
            method: 'PUT',
            headers: { 'Authorization': `Bearer ${accessToken}` },
            body: formData
        });

        if (response.ok) {
            alert('Funcionário atualizado com sucesso!');
        } else {
            const errorText = await response.text();
            throw new Error(`Falha ao atualizar: ${errorText}`);
        }
    } catch (error) {
        console.error('Erro ao salvar alterações:', error);
        alert(error.message);
    } finally {
        delete originalRowHTML[employeeId];
        loadEmployees();
    }
};

window.cancelEdit = (employeeId) => {
    const row = document.getElementById(`row-${employeeId}`);
    if (row && originalRowHTML[employeeId]) {
        row.innerHTML = originalRowHTML[employeeId];
        delete originalRowHTML[employeeId];
    }
    document.querySelectorAll('.btn-edit').forEach(btn => btn.disabled = false);
};

// =================================================================
// FUNÇÕES DO FORMULÁRIO DE CADASTRO (POST)
// =================================================================

function waitForForm() {
    // Usamos um seletor mais específico para o formulário de cadastro
    const employeeForm = document.querySelector('form.employee-form');
    if (!employeeForm) {
        // Se o form de cadastro não estiver na tela, não há nada a fazer.
        console.log('Formulário de cadastro de funcionário não encontrado nesta visão.');
        return;
    }
    initializeForm(employeeForm);
}

function initializeForm(employeeForm) {
    console.log('🚀 Inicializando formulário de cadastro de funcionário...');
    const handleSaveEmployee = async (event) => {
        event.preventDefault();
        try {
            const accessToken = localStorage.getItem('accessToken');
            if (!accessToken) {
                alert('Você não está autenticado.');
                return;
            }
            const formData = new FormData(employeeForm);
            if (!formData.get('Name') || !formData.get('CPF') || !formData.get('Position')) {
                alert('Por favor, preencha Nome, CPF e Cargo.');
                return;
            }
            
            // Renomeia o campo 'Position' para 'Positiions' antes de enviar
            formData.append('Positiions', formData.get('Position'));
            formData.delete('Position');

            const response = await fetch(API_URL, {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${accessToken}` },
                body: formData,
            });
            
            if (response.ok) {
                alert('Funcionário salvo com sucesso!');
                employeeForm.reset();
                loadEmployees(); // Atualiza a tabela
            } else {
                const errorText = await response.text();
                throw new Error(`Erro ao salvar (Status ${response.status}): ${errorText}`);
            }
        } catch (error) {
            console.error('Erro no handleSaveEmployee:', error);
            alert(error.message);
        }
    };
    employeeForm.addEventListener('submit', handleSaveEmployee);
}

// --- EXECUÇÃO PRINCIPAL (CORRIGIDA) ---
// Esta função será chamada pela 'loadForm' depois que o script for carregado.
function initDynamicForm() {
    console.log('▶️ initDynamicForm() de employee.js foi chamada.');
    // Tenta inicializar o formulário de cadastro, se ele existir na página.
    waitForForm();
    // Carrega a lista de funcionários na tabela, se ela existir na página.
    loadEmployees();
}