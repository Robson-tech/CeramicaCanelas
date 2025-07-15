function loadForm(formName) {
    // PROTEÇÃO: Só executa se foi um clique real
    if (!event || (event.type !== 'click' && event.type !== 'keydown')) {
        console.warn("🚫 BLOQUEADO: loadForm() só pode ser chamada por cliques ou teclas!");
        console.trace("❌ Tentativa de chamada automática bloqueada:");
        return;
    }
    
    console.log("✅ PERMITIDO: Chamada via", event.type);
    
    const container = document.getElementById('form-container');
    
    if (!container) {
        console.error("❌ ERRO: Elemento 'form-container' não encontrado!");
        return;
    }
    
    const welcomeMessage = document.getElementById('welcome-message');
    if (welcomeMessage) {
        welcomeMessage.style.display = 'none';
    }

    // Limpa o conteúdo anterior e remove o script antigo
    container.innerHTML = '';
    const oldScript = document.getElementById('dynamic-form-script');
    if (oldScript) {
        oldScript.remove();
    }

    fetch(`/forms/${formName}.html`)
        .then(response => {
            if (!response.ok) {
                throw new Error(`Erro ao carregar o formulário: ${response.statusText}`);
            }
            return response.text();
        })
        .then(html => {
            container.innerHTML = html;
            
            // Aguarda o HTML ser renderizado no DOM
            setTimeout(() => {
                // Verifica se o formulário foi inserido antes de carregar o script
                const formCheck = container.querySelector('form');
                if (!formCheck) {
                    console.error('❌ ERRO: Formulário não foi inserido no DOM!');
                    return;
                }
                
                console.log('✅ HTML do formulário inserido. Carregando script...');
                
                const script = document.createElement('script');
                script.id = 'dynamic-form-script';
                script.src = `/js/${formName}.js`;
                script.defer = true;
                
                script.onload = () => {
                    console.log(`✅ Script ${formName}.js carregado com sucesso`);
                };
                
                script.onerror = () => {
                    console.error(`❌ Erro ao carregar o script ${formName}.js`);
                };
                
                document.body.appendChild(script);
            }, 200); // Aumentei para 200ms para garantir
        })
        .catch(error => {
            console.error('💥 Erro no processo de loadForm:', error);
            container.innerHTML = `<p style="color:red;">Erro: ${error.message}</p>`;
        });
}