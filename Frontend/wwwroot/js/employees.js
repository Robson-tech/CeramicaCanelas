console.log('✅ SCRIPT: js/employee.js foi carregado e está executando.');

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
        // Para DELETE, a API deve usar o ID na URL
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

// --- Funções de Edição na Tabela (ATUALIZADAS) ---

/**
 * ATUALIZADO: Adiciona um campo de upload de imagem durante a edição.
 */
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

    // Adiciona um campo para selecionar uma nova imagem
    imageCell.innerHTML = `${currentImageHTML}<br><label style="font-size: 12px; margin-top: 5px; display: block;">Trocar Imagem:<input type="file" class="edit-file" accept="image/*"></label>`;
    
    // Transforma as outras células em campos editáveis
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

/**
 * ATUALIZADO: Envia os dados como 'multipart/form-data' e o ID no corpo.
 */
window.saveEmployeeChanges = async (employeeId) => {
    const row = document.getElementById(`row-${employeeId}`);
    if (!row) return;

    const accessToken = localStorage.getItem('accessToken');
    if (!accessToken) {
        alert('Autenticação necessária.');
        return;
    }

    // 1. Cria um objeto FormData para enviar os dados
    const formData = new FormData();

    // 2. Pega os valores dos campos
    const nameValue = row.querySelector('[data-field="name"] input').value;
    const cpfValue = row.querySelector('[data-field="cpf"] input').value;
    const positionValue = row.querySelector('[data-field="position"] select').value;
    const imageFile = row.querySelector('.edit-file').files[0];

    // 3. Adiciona todos os campos ao FormData, INCLUINDO O ID
    formData.append('Id', employeeId);
    formData.append('Name', nameValue);
    formData.append('CPF', cpfValue);
    formData.append('Positiions', positionValue);
    
    // Apenas anexa a imagem se o usuário selecionou um novo arquivo
    if (imageFile) {
        formData.append('Imagem', imageFile);
    }

    try {
        // 4. Faz a requisição PUT para a URL base, sem o ID
        const response = await fetch(API_URL, {
            method: 'PUT',
            headers: {
                // Ao usar FormData, NÃO definimos o 'Content-Type'. O navegador faz isso.
                'Authorization': `Bearer ${accessToken}`
            },
            body: formData // O corpo da requisição é o objeto FormData
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
        loadEmployees(); // Recarrega a tabela para mostrar o estado atualizado
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
// FUNÇÕES DO FORMULÁRIO DE CADASTRO (POST) - Sem alterações
// =================================================================

function waitForForm() {
    const employeeForm = document.querySelector('.employee-form');
    if (!employeeForm) {
        setTimeout(waitForForm, 100);
        return;
    }
    initializeForm(employeeForm);
}

function initializeForm(employeeForm) {
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
            const positionValue = parseInt(formData.get('Position'), 10);
            if (isNaN(positionValue)) {
                alert('Por favor, selecione um cargo válido.');
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
                loadEmployees();
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

// --- EXECUÇÃO PRINCIPAL ---
waitForForm();
loadEmployees();