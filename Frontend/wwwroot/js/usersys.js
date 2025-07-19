console.log('Script js/usersys.js DEFINIDO.');

// Este script utiliza as variáveis e funções globais de main.js
// como API_BASE_URL e showErrorModal.

// =======================================================
// INICIALIZAÇÃO DA PÁGINA
// =======================================================
function initDynamicForm() {
    console.log('▶️ initDynamicForm() de usersys.js foi chamada.');
    const formElement = document.querySelector('.user-form');
    initializeUserForm(formElement);
}

// =======================================================
// LÓGICA DO FORMULÁRIO DE CADASTRO
// =======================================================

/**
 * Anexa o evento de submit ao formulário.
 */
function initializeUserForm(userForm) {
    if (!userForm) {
        console.error('FALHA CRÍTICA: Elemento <form class="user-form"> não encontrado.');
        return;
    }
    console.log('🚀 Inicializando formulário de usuário...');
    
    userForm.onsubmit = (event) => {
        event.preventDefault(); // Impede o recarregamento da página
        processUserData(userForm);
    };
    
    console.log('✅ Event listener do formulário configurado com sucesso!');
}

/**
 * Processa e valida os dados do usuário antes do envio.
 */
async function processUserData(form) {
    console.log('🔍 Validando dados do usuário...');
    
    const formData = new FormData(form);
    const password = formData.get('password');
    const passwordConfirmation = formData.get('passwordConfirmation');
    
    if (password !== passwordConfirmation) {
        showErrorModal({ title: "Validação Falhou", detail: "As senhas não coincidem. Verifique e tente novamente."});
        return;
    }
    
    if (password.length < 6) {
        showErrorModal({ title: "Validação Falhou", detail: "A senha deve ter pelo menos 6 caracteres."});
        return;
    }
    
    const requiredFields = ['userName', 'name', 'email', 'password', 'role'];
    for (const field of requiredFields) {
        if (!formData.get(field)) {
            showErrorModal({ title: "Validação Falhou", detail: "Por favor, preencha todos os campos obrigatórios."});
            return;
        }
    }
    
    console.log('✅ Dados validados com sucesso.');
    await sendUserData(formData, form);
}

/**
 * Envia os dados para a API para criar um novo usuário.
 */
async function sendUserData(formData, form) {
    console.log('📡 Preparando dados para envio...');
    
    // Remove o campo de confirmação, que não é necessário no backend
    formData.delete('passwordConfirmation');
    
    // O corpo da requisição será 'application/x-www-form-urlencoded'
    const body = new URLSearchParams(formData);

    try {
        const accessToken = localStorage.getItem('accessToken');
        const url = `${API_BASE_URL}/user`; // Supondo que este é o endpoint de criação de usuário
        
        const response = await fetch(url, {
            method: 'POST',
            headers: { 
                'Authorization': `Bearer ${accessToken}`,
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: body
        });

        if (response.ok) {
            alert('Usuário cadastrado com sucesso!');
            form.reset();
            // Futuramente, pode chamar uma função para recarregar uma tabela de usuários
            // loadUsers(); 
        } else {
            const errorData = await response.json().catch(() => ({ title: `Erro ${response.status}`, detail: "Não foi possível processar a requisição." }));
            showErrorModal(errorData);
        }
    } catch (error) {
        console.error('❌ Erro na requisição:', error);
        showErrorModal({ title: "Erro de Conexão", detail: "Falha na comunicação com o servidor." });
    }
}