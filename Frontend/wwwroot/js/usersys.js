// LOG 1: Confirma que o arquivo de script foi carregado e está sendo executado.
console.log('Script usersys.js (somente cadastro) EXECUTANDO.');



// Função principal que inicializa o formulário
function initializeUserForm(userForm) {
    if (!userForm) {
        console.error('FALHA CRÍTICA: Elemento <form class="user-form"> não encontrado.');
        return;
    }

    console.log('🚀 Inicializando formulário de usuário...');
    
    userForm.addEventListener('submit', (event) => {
        console.log('EVENTO "submit" capturado! Impedindo recarregamento da página.');
        event.preventDefault(); // Impede o recarregamento da página
        console.log('Iniciando validação e processamento dos dados...');
        processUserData(userForm);
    });
    
    console.log('✅ Event listener do formulário configurado com sucesso!');
}

// Função para processar e validar os dados do usuário
async function processUserData(form) {
    console.log('🔍 Iniciando validação dos dados...');
    
    const formData = new FormData(form);
    const password = formData.get('password');
    const passwordConfirmation = formData.get('passwordConfirmation');
    
    if (password !== passwordConfirmation) {
        console.warn('⚠️ Senhas não coincidem');
        alert('As senhas não coincidem. Verifique e tente novamente.');
        return;
    }
    
    if (password.length < 6) {
        console.warn('⚠️ Senha muito curta');
        alert('A senha deve ter pelo menos 6 caracteres.');
        return;
    }
    
    // Usamos diretamente o formData que é mais prático para application/x-www-form-urlencoded
    const requiredFields = ['userName', 'name', 'email', 'password', 'role'];
    for (const field of requiredFields) {
        if (!formData.get(field)) {
            console.warn(`⚠️ Campo obrigatório não preenchido: ${field}`);
            alert('Por favor, preencha todos os campos obrigatórios.');
            return;
        }
    }
    
    console.log('✅ Dados validados com sucesso');
    await sendUserData(formData, form);
}

// Função para enviar os dados para a API
async function sendUserData(formData, form) {
    console.log('📡 Preparando dados para envio...');
    // Clonamos para poder logar sem a senha, o original vai no body
    const logData = new FormData(form);
    logData.set('password', '[OCULTO]');
    logData.set('passwordConfirmation', '[OCULTO]');
    console.log('Enviando para a API:', Object.fromEntries(logData));

    try {
        // A URL agora usa a variável API_BASE_URL e não contém os dados
        const url = `${API_BASE_URL}/api/user`;
        
        const response = await fetch(url, {
            method: 'POST',
            headers: { 
                // O Content-Type correto para FormData via fetch é omitido, 
                // o navegador define com o boundary correto.
                // Mas para x-www-form-urlencoded, usamos URLSearchParams.
                'Accept': 'application/json'
            },
            // Os dados são enviados no CORPO da requisição, não na URL.
            body: new URLSearchParams(formData)
        });

        if (response.ok) {
            console.log('✅ Usuário salvo com sucesso!');
            alert('Usuário cadastrado com sucesso!');
            form.reset(); // Limpa o formulário
        } else {
            let errorMessage = 'Erro ao salvar usuário';
            try {
                const errorData = await response.json();
                errorMessage = errorData.message || errorData.title || JSON.stringify(errorData);
            } catch {
                errorMessage = `Erro ${response.status}: ${response.statusText}`;
            }
            console.error('❌ Erro da API:', errorMessage);
            alert(`Erro ao salvar usuário: ${errorMessage}`);
        }
    } catch (error) {
        console.error('❌ Erro na requisição:', error);
        alert('Falha na comunicação com o servidor. Verifique se a API está rodando.');
    }
}

// --- EXECUÇÃO PRINCIPAL ---
const formElement = document.querySelector('.user-form');
initializeUserForm(formElement);