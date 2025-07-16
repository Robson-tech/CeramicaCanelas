/**
 * Carrega dinamicamente um formulário e seu script correspondente.
 * VERSÃO FINAL CORRIGIDA E ROBUSTA
 */
function loadForm(formName) {
    // A verificação problemática do 'event' foi removida.
    // A função agora confia que será chamada corretamente pelo 'onclick' no HTML.
    
    console.log(`▶️ Iniciando carregamento do formulário: ${formName}`);
    
    const container = document.getElementById('form-container');
    if (!container) {
        console.error("❌ ERRO: Elemento 'form-container' não encontrado!");
        return;
    }
    
    const welcomeMessage = document.getElementById('welcome-message');
    if (welcomeMessage) {
        welcomeMessage.style.display = 'none';
    }

    // Limpa o conteúdo e o script antigo
    container.innerHTML = '<h2>Carregando Formulário...</h2>';
    const oldScript = document.getElementById('dynamic-form-script');
    if (oldScript) {
        oldScript.remove();
    }

    // Busca o arquivo HTML do formulário
    fetch(`/forms/${formName}.html`)
        .then(response => {
            if (!response.ok) throw new Error(`Formulário ${formName}.html não encontrado.`);
            return response.text();
        })
        .then(html => {
            container.innerHTML = html;
            
            // Após inserir o HTML, cria e carrega o script associado
            const script = document.createElement('script');
            script.id = 'dynamic-form-script';
            script.src = `/js/${formName}.js`;
            
            script.onload = () => {
                console.log(`✅ Script ${formName}.js carregado com sucesso.`);
                
                // A "ponte" que chama a função de inicialização do script recém-carregado
                if (typeof window.initDynamicForm === 'function') {
                    console.log(`🚀 Executando initDynamicForm() de ${formName}.js`);
                    window.initDynamicForm();

                    // --- MELHORIA ADICIONADA ---
                    // Limpa a função global depois de usá-la para evitar conflitos futuros.
                    delete window.initDynamicForm;
                    console.log('🧹 Função initDynamicForm() limpa do escopo global.');
                    
                } else {
                    console.warn(`⚠️ AVISO: O script ${formName}.js não possui a função initDynamicForm().`);
                }
            };
            
            script.onerror = () => {
                console.error(`❌ Erro fatal ao carregar o script ${formName}.js`);
            };
            
            document.body.appendChild(script);
        })
        .catch(error => {
            console.error('💥 Erro no processo de loadForm:', error);
            container.innerHTML = `<p style="color:red; text-align:center;">${error.message}</p>`;
        });
}