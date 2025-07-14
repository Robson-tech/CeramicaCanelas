console.log('Script js/category.js (somente cadastro com auth) EXECUTANDO.');

document.addEventListener('DOMContentLoaded', () => {
    console.log('✅ DOM totalmente carregado');

    const API_URL = 'http://localhost:5087/api/categories';
    const categoryForm = document.querySelector('.category-form');
    const btnSalvar = document.getElementById('btnSalvar');

    if (!categoryForm) {
        console.error('❌ Formulário .category-form não foi encontrado!');
        return;
    }

    /**
     * Função para salvar categoria com autenticação
     */
    const handleSaveCategory = async (event) => {
        event.preventDefault(); // 🚫 impede recarregar a página
        console.log('📌 handleSaveCategory chamado');

        // 1. Obter o token do localStorage
        const accessToken = localStorage.getItem('accessToken');
        console.log('🔑 Token carregado?', !!accessToken);

        // 2. Verificar se o token existe
        if (!accessToken) {
            alert('Você não está autenticado. Por favor, faça o login novamente.');
            console.error('❌ Access token não encontrado no localStorage.');
            return; // Interrompe a execução se não houver token
        }

        const nameInput = categoryForm.querySelector('[name="categoryName"]');
        const descriptionInput = categoryForm.querySelector('[name="categoryDescription"]');
        const imageInput = categoryForm.querySelector('[name="categoryImage"]');

        if (!nameInput.value.trim()) {
            alert('Preencha o nome da categoria!');
            return;
        }

        const formData = new FormData();
        formData.append('Name', nameInput.value);
        formData.append('Description', descriptionInput.value);

        if (imageInput.files[0]) {
            formData.append('ImageFile', imageInput.files[0]);
        }

        console.log('📦 Dados prontos para envio:', {
            Name: nameInput.value,
            Description: descriptionInput.value,
            Image: imageInput.files[0] ? imageInput.files[0].name : 'Nenhuma imagem'
        });

        try {
            const response = await fetch(API_URL, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${accessToken}`
                    // ❗ NÃO adicionar Content-Type aqui, pois o fetch detecta automaticamente quando é FormData
                },
                body: formData,
            });

            console.log('📡 Status da resposta:', response.status);

            if (response.status === 401) {
                alert('Sua sessão expirou ou o token é inválido. Faça login novamente.');
                console.warn('⚠️ Token expirado ou inválido.');
                return;
            }

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({ message: 'Erro desconhecido ao salvar.' }));
                throw new Error(errorData.message || 'Não foi possível salvar a categoria.');
            }

            alert('✅ Categoria salva com sucesso!');
            categoryForm.reset();

            // Opcional: atualizar a lista de categorias dinamicamente
            // carregarCategorias();

        } catch (error) {
            console.error('❌ Falha ao salvar categoria:', error);
            alert(`Erro: ${error.message}`);
        }
    };

    // --- INICIALIZAÇÃO ---
    // Impede comportamento padrão do form e chama a função manualmente
    categoryForm.addEventListener('submit', handleSaveCategory);

    // Também conecta o botão manualmente (para evitar qualquer submit automático)
    if (btnSalvar) {
        btnSalvar.addEventListener('click', handleSaveCategory);
    }
});
