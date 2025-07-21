console.log('Script js/usersys.js DEFINIDO.');


// =======================================================
// INICIALIZAÇÃO DA PÁGINA
// =======================================================
function initDynamicForm() {
    console.log('▶️ initDynamicForm() de usersys.js foi chamada.');
    const formElement = document.querySelector('.user-form');
    initializeUserForm(formElement);
    fetchAndRenderUsers();
}

// =======================================================
// LÓGICA DO FORMULÁRIO DE CADASTRO
// =======================================================
function initializeUserForm(userForm) {
    if (!userForm) return;
    userForm.onsubmit = (event) => {
        event.preventDefault();
        processUserData(userForm);
    };
}

async function processUserData(form) {
    const formData = new FormData(form);
    const password = formData.get('password');
    const passwordConfirmation = formData.get('passwordConfirmation');
    if (password !== passwordConfirmation) {
        showErrorModal({ title: "Validação Falhou", detail: "As senhas não coincidem."});
        return;
    }
    const requiredFields = ['userName', 'name', 'email', 'password', 'role'];
    for (const field of requiredFields) {
        if (!formData.get(field)) {
            showErrorModal({ title: "Validação Falhou", detail: "Por favor, preencha todos os campos obrigatórios."});
            return;
        }
    }
    await sendUserData(formData, form);
}

async function sendUserData(formData, form) {
    formData.delete('passwordConfirmation');
    try {
        const accessToken = localStorage.getItem('accessToken');
        const url = `${API_BASE_URL}/user`;
        const response = await fetch(url, {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${accessToken}` },
            body: formData
        });
        if (response.ok) {
            alert('Usuário cadastrado com sucesso!');
            form.reset();
            fetchAndRenderUsers();
        } else {
            const errorData = await response.json().catch(() => ({ title: `Erro ${response.status}` }));
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: "Falha na comunicação com o servidor." });
    }
}

// =======================================================
// LÓGICA DA TABELA DE LISTAGEM E EXCLUSÃO
// =======================================================
async function fetchAndRenderUsers() {
    const tableBody = document.querySelector('#user-list-body');
    if (!tableBody) return;
    tableBody.innerHTML = '<tr><td colspan="5" style="text-align: center;">Buscando usuários...</td></tr>';
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/user`, { headers: { 'Authorization': `Bearer ${accessToken}` } });
        if (!response.ok) throw new Error('Falha ao buscar a lista de usuários.');
        const data = await response.json();
        const users = data.items;
        if (!Array.isArray(users)) {
            throw new Error("A resposta da API não continha uma lista de 'items' válida.");
        }
        renderUserTable(users, tableBody);
    } catch (error) {
        showErrorModal({ title: "Erro ao Listar", detail: error.message });
        tableBody.innerHTML = `<tr><td colspan="5" style="text-align: center; color: red;">${error.message}</td></tr>`;
    }
}

function renderUserTable(users, tableBody) {
    tableBody.innerHTML = '';
    if (!users || users.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="5" style="text-align: center;">Nenhum usuário cadastrado.</td></tr>';
        return;
    }

    // ATUALIZADO: A função agora usa o 'userRolesMap'
    const getRoleName = (roleId) => userRolesMap[roleId] || 'Desconhecido';

    users.forEach(user => {
        const rowHTML = `
            <tr id="row-user-${user.id}">
                <td>${user.userName}</td>
                <td>${user.name}</td>
                <td>${user.email}</td>
                <td>${getRoleName(user.role)}</td>
                <td class="actions-cell">
                    <button class="btn-action btn-delete" onclick="deleteUser('${user.id}')">Excluir</button>
                </td>
            </tr>`;
        tableBody.insertAdjacentHTML('beforeend', rowHTML);
    });
}

window.deleteUser = async (userId) => {
    if (!confirm('Tem certeza que deseja excluir este usuário?')) return;
    try {
        const accessToken = localStorage.getItem('accessToken');
        const response = await fetch(`${API_BASE_URL}/user/${userId}`, {
            method: 'DELETE',
            headers: { 'Authorization': `Bearer ${accessToken}` }
        });
        if (response.ok) {
            alert('Usuário excluído com sucesso!');
            fetchAndRenderUsers();
        } else {
            const errorData = await response.json().catch(() => ({ title: "Erro ao Excluir" }));
            showErrorModal(errorData);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: error.message });
    }
};