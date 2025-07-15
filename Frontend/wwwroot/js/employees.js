console.log('✅ SCRIPT: js/employee.js foi carregado e está executando.');

// Função para aguardar o formulário aparecer no DOM
function waitForForm() {
    console.log('🔍 Procurando o formulário de funcionário...');
    
    const employeeForm = document.querySelector('.employee-form');
    
    if (!employeeForm) {
        console.log('⏳ Formulário ainda não encontrado. Tentando novamente em 100ms...');
        setTimeout(waitForForm, 100);
        return;
    }
    
    console.log('👍 SUCESSO: Formulário .employee-form encontrado!', employeeForm);
    initializeForm(employeeForm);
}

// Função para inicializar o formulário
function initializeForm(employeeForm) {
    const API_URL = 'http://localhost:5087/api/employees';

    /**
     * Manipula o envio do formulário para salvar um novo funcionário.
     */
    const handleSaveEmployee = async (event) => {
        console.log('🚀 EVENTO SUBMIT CAPTURADO!');
        console.log('📋 Event object:', event);
        console.log('📋 Event type:', event.type);
        console.log('📋 Event target:', event.target);
        
        // MUITO IMPORTANTE: Impede o comportamento padrão IMEDIATAMENTE
        event.preventDefault();
        event.stopPropagation();
        event.stopImmediatePropagation();
        
        console.log('✅ preventDefault() executado com sucesso!');

        try {
            const accessToken = localStorage.getItem('accessToken');
            if (!accessToken) {
                alert('Você não está autenticado. Por favor, faça o login novamente.');
                return;
            }

            const formData = new FormData(employeeForm);
            console.log('📝 FormData criado:', formData);

            // Debug: mostra todos os valores do form
            for (let [key, value] of formData.entries()) {
                console.log(`📋 ${key}:`, value);
            }

            if (!formData.get('Name') || !formData.get('CPF') || !formData.get('Position')) {
                alert('Por favor, preencha Nome, CPF e Cargo.');
                return;
            }

            // Converte a posição para número inteiro
            const positionValue = parseInt(formData.get('Position'), 10);
            console.log('🔢 Posição convertida:', positionValue);
            
            if (isNaN(positionValue)) {
                alert('Por favor, selecione um cargo válido.');
                return;
            }

            const imageFile = formData.get('ImageFile');
            console.log('🖼️ Arquivo de imagem:', imageFile);
            
            // Se há imagem, usa FormData (multipart)
            if (imageFile && imageFile.size > 0) {
                const finalFormData = new FormData();
                finalFormData.append('Name', formData.get('Name'));
                finalFormData.append('CPF', formData.get('CPF'));
                finalFormData.append('Positiions', positionValue.toString());
                finalFormData.append('ImageFile', imageFile);

                console.log('📡 Enviando dados com imagem (FormData)...');
                console.log('📊 Posição selecionada:', positionValue);
                
                const response = await fetch(API_URL, {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer ${accessToken}`
                    },
                    body: finalFormData,
                });

                console.log('📡 Response status:', response.status);
                console.log('📡 Response ok:', response.ok);

                if (response.status === 401) {
                    alert('Sua sessão expirou ou o token é inválido. Faça o login novamente.');
                    return;
                }

                if (response.ok) {
                    alert('Funcionário salvo com sucesso!');
                    employeeForm.reset();
                } else {
                    // Captura detalhes do erro da API
                    const errorText = await response.text();
                    console.error('📡 Erro da API (texto):', errorText);
                    
                    try {
                        const errorData = JSON.parse(errorText);
                        console.error('📡 Erro da API (JSON):', errorData);
                        throw new Error(errorData.message || errorData.title || 'Erro desconhecido da API');
                    } catch (parseError) {
                        console.error('📡 Erro ao parsear JSON:', parseError);
                        throw new Error(`Erro da API: ${errorText}`);
                    }
                }
            } else {
                // Sem imagem, usa JSON para manter tipos corretos
                const jsonData = {
                    Name: formData.get('Name'),
                    CPF: formData.get('CPF'),
                    Positions: positionValue // Mantém como número
                };

                console.log('📡 Enviando dados sem imagem (JSON)...');
                console.log('📊 Dados enviados:', jsonData);
                
                const response = await fetch(API_URL, {
                    method: 'POST',
                    headers: {
                        'Authorization': `Bearer ${accessToken}`,
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(jsonData),
                });

                console.log('📡 Response status:', response.status);
                console.log('📡 Response ok:', response.ok);

                if (response.status === 401) {
                    alert('Sua sessão expirou ou o token é inválido. Faça o login novamente.');
                    return;
                }

                if (response.ok) {
                    alert('Funcionário salvo com sucesso!');
                    employeeForm.reset();
                } else {
                    // Captura detalhes do erro da API
                    const errorText = await response.text();
                    console.error('📡 Erro da API (texto):', errorText);
                    
                    try {
                        const errorData = JSON.parse(errorText);
                        console.error('📡 Erro da API (JSON):', errorData);
                        throw new Error(errorData.message || errorData.title || 'Erro desconhecido da API');
                    } catch (parseError) {
                        console.error('📡 Erro ao parsear JSON:', parseError);
                        throw new Error(`Erro da API: ${errorText}`);
                    }
                }
            }
        } catch (error) {
            console.error('💥 Erro completo:', error);
            console.error('💥 Stack trace:', error.stack);
            alert(`Erro: ${error.message}`);
        }
    };

    console.log('🔗 Anexando o "escutador" de evento "submit" ao formulário.');
    
    // Adiciona múltiplos listeners para garantir interceptação
    employeeForm.addEventListener('submit', handleSaveEmployee, true); // Captura
    employeeForm.addEventListener('submit', handleSaveEmployee, false); // Bubble
    
    // Também intercepta o botão diretamente
    const submitButton = employeeForm.querySelector('button[type="submit"]');
    if (submitButton) {
        console.log('🔘 Botão submit encontrado!');
        submitButton.addEventListener('click', (e) => {
            console.log('🖱️ Clique no botão interceptado!');
            // Força a execução do handler do form
            handleSaveEmployee(e);
        });
    }
    
    console.log('✅ Todos os event listeners anexados com sucesso!');
}

// Inicia a busca pelo formulário
waitForForm();