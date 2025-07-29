console.log('Script js/usersys.js DEFINIDO.');

/**
 * Traduz mensagens de erro da API do inglês para o português.
 * @param {string} mensagemIngles - A mensagem original retornada pela API.
 * @returns {string} A mensagem traduzida ou a original se não houver tradução.
 */
function traduzirMensagemApi(mensagemIngles) {
    // Dicionário para erros com texto exato. É mais rápido.
    const errosExatos = {
        "Passwords must have at least one non alphanumeric character.": "As senhas devem ter pelo menos um caractere especial (ex: !, @, #).",
        "Passwords must have at least one uppercase ('A'-'Z').": "As senhas devem ter pelo menos uma letra maiúscula ('A'-'Z').",
        "O campo de confirmação de senha deve ser igual ao campo senha": "O campo de confirmação de senha deve ser igual ao campo Senha."
    };

    // 1. Verifica se há uma tradução exata e a retorna imediatamente.
    if (errosExatos[mensagemIngles]) {
        return errosExatos[mensagemIngles];
    }

    // Dicionário para erros com padrões dinâmicos (regex)
    const errosPadrao = {
        [/^Username '(.+)' is already taken\.$/]: (valor) => `O nome de usuário '${valor}' já está em uso.`,
        [/^User with email (.+) already exists\.$/]: (valor) => `O e-mail '${valor}' já foi cadastrado.`
    };

    // 2. Se não encontrou um erro exato, itera sobre os padrões.
    for (const padrao in errosPadrao) {
        const regex = new RegExp(padrao.slice(1, -1)); // Converte a chave para Regex
        const match = mensagemIngles.match(regex);
        
        if (match) {
            const valorDinamico = match[1];
            const tradutor = errosPadrao[padrao];
            return tradutor(valorDinamico);
        }
    }

    // 3. Se nenhuma tradução foi encontrada, retorna a mensagem original.
    return mensagemIngles;
}


/**
 * Transforma a resposta de erro da API em um formato amigável para exibição no modal.
 */
function parseApiError(errorData) {
    if (errorData && typeof errorData.message === 'string') {
        const mensagemTraduzida = traduzirMensagemApi(errorData.message);
        
        return {
            title: "Falha na Requisição",
            detail: mensagemTraduzida
        };
    }
    if (errorData && typeof errorData.errors === 'object' && errorData.errors !== null) {
        let detailsHtml = '<ul style="text-align: left; padding-left: 20px;">';
        for (const key in errorData.errors) {
            const messages = errorData.errors[key];
            if (Array.isArray(messages)) {
                messages.forEach(msg => {
                    detailsHtml += `<li>${traduzirMensagemApi(msg)}</li>`;
                });
            }
        }
        detailsHtml += '</ul>';
        return {
            title: "Falha na Validação",
            detail: detailsHtml
        };
    }
    return {
        title: "Erro Desconhecido",
        detail: "A operação não pôde ser concluída."
    };
}


function initDynamicForm() {
    console.log('▶️ initDynamicForm() de usersys.js foi chamada.');
    const formElement = document.querySelector('.user-form');
    initializeUserForm(formElement);
    fetchAndRenderUsers();
}

function initializeUserForm(userForm) {
    if (!userForm) return;
    userForm.onsubmit = (event) => {
        event.preventDefault();
        processUserData(userForm);
    };
}

async function processUserData(form) {
    const formData = new FormData(form);
    const password = formData.get('Password')?.trim() || '';
    const passwordConfirmation = formData.get('PasswordConfirmation')?.trim() || '';

    if (password !== passwordConfirmation) {
        showErrorModal({ title: "Validação Falhou", detail: "As senhas não coincidem."});
        return;
    }

    const requiredFields = ['UserName', 'Name', 'Email', 'Password', 'Role'];
    for (const field of requiredFields) {
        if (!formData.get(field)) {
            showErrorModal({ title: "Validação Falhou", detail: `O campo obrigatório '${field}' não foi preenchido.`});
            return;
        }
    }
    await sendUserData(formData, form);
}

async function sendUserData(formData, form) {
    const submitButton = form.querySelector('.submit-btn');

    if (!submitButton) {
        console.error("ERRO: Botão com a classe '.submit-btn' não foi encontrado no HTML.");
        return;
    }

    const originalButtonHTML = submitButton.innerHTML;
    submitButton.disabled = true;
    submitButton.innerHTML = `<span class="loading-spinner"></span> Salvando...`;

    try {
        const accessToken = localStorage.getItem('accessToken');
        const url = `${API_BASE_URL}/user`;

        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${accessToken}`
            },
            body: formData
        });

        if (response.ok) {
            alert('Usuário cadastrado com sucesso!');
            form.reset();
            fetchAndRenderUsers();
        } else {
            const errorData = await response.json().catch(() => ({
                title: `Erro ${response.status}`,
                message: "A resposta do servidor não é um JSON válido ou está vazia."
            }));
            
            console.log("🔴 CORPO DO ERRO DA API:", errorData);
            const formattedError = parseApiError(errorData);
            showErrorModal(formattedError);
        }
    } catch (error) {
        console.error('❌ Erro na requisição:', error);
        showErrorModal({ title: "Erro de Conexão", detail: "Falha na comunicação com o servidor." });
    } finally {
        submitButton.disabled = false;
        submitButton.innerHTML = originalButtonHTML;
    }
}

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
    
    users.forEach(user => {
        const rolesText = Array.isArray(user.roles) && user.roles.length > 0
            ? user.roles.join(', ')
            : 'Nenhum papel';

        const rowHTML = `
            <tr id="row-user-${user.id}">
                <td>${user.userName}</td>
                <td>${user.name}</td>
                <td>${user.email}</td>
                <td>${rolesText}</td>
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
            const formattedError = parseApiError(errorData);
            showErrorModal(formattedError);
        }
    } catch (error) {
        showErrorModal({ title: "Erro de Conexão", detail: error.message });
    }
};