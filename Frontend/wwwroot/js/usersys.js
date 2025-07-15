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
    
    const userData = {
        UserName: formData.get('userName'),
        Name: formData.get('name'),
        Email: formData.get('email'),
        Password: password,
        PasswordConfirmation: passwordConfirmation,
        Role: parseInt(formData.get('role'))
    };
    
    if (!userData.UserName || !userData.Name || !userData.Email || isNaN(userData.Role)) {
        console.warn('⚠️ Campos obrigatórios não preenchidos');
        alert('Por favor, preencha todos os campos obrigatórios.');
        return;
    }
    
    console.log('✅ Dados validados com sucesso');
    await sendUserData(userData, form);
}

// Função para enviar os dados para a API
async function sendUserData(userData, form) {
    console.log('📡 Preparando dados para envio...');
    console.log('Enviando para a API:', { ...userData, Password: '[OCULTO]', PasswordConfirmation: '[OCULTO]' });

    try {
        const params = new URLSearchParams(userData);
        
        const response = await fetch(`http://localhost:5087/api/user?${params.toString()}`, {
            method: 'POST',
            headers: { 
                'Content-Type': 'application/x-www-form-urlencoded',
                'Accept': 'application/json'
            }
        });

        if (response.ok) {
            console.log('✅ Usuário salvo com sucesso!');
            alert('Usuário cadastrado com sucesso!');
            form.reset(); // Limpa o formulário
            // A linha para recarregar a tabela foi removida daqui.
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
// Como este script é carregado DEPOIS do HTML, podemos buscar os elementos diretamente.
const formElement = document.querySelector('.user-form');
initializeUserForm(formElement);
// A chamada para loadUsers() foi removida do final do arquivo.