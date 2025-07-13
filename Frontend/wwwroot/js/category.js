    document.addEventListener('DOMContentLoaded', () => {
      console.log('DEBUG: DOM carregado.');

      const btnSalvar = document.getElementById('btnSalvar');
      if (!btnSalvar) {
        console.error('ERRO: Botão de salvar não encontrado!');
        return;
      } else {
        console.log('DEBUG: Botão de salvar encontrado.');
      }

      const categoryForm = document.querySelector('.category-form');
      if (!categoryForm) {
        console.error('ERRO: Formulário com a classe .category-form não encontrado!');
        return;
      } else {
        console.log('DEBUG: Formulário encontrado.');
      }

      btnSalvar.addEventListener('click', async () => {
        console.log('DEBUG: Botão salvar clicado.');

        const categoryName = categoryForm.querySelector('input[name="categoryName"]').value.trim();
        const categoryDescription = categoryForm.querySelector('textarea[name="categoryDescription"]').value.trim();

        console.log('DEBUG: Valores capturados:', { categoryName, categoryDescription });

        if (!categoryName) {
          alert('O nome da categoria é obrigatório.');
          return;
        }

        const data = {
          name: categoryName,
          description: categoryDescription,
          imageUrl: ''
        };

        try {
          console.log('DEBUG: Enviando dados para a API...', data);

          const response = await fetch('http://localhost:5087/api/categories', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
          });

          console.log('DEBUG: Resposta da API:', response.status);

          if (response.ok) {
            alert('Categoria salva com sucesso!');
            categoryForm.reset();
            // Aqui você adicionaria a lógica para recarregar a tabela ou adicionar a nova linha
          } else {
            const errorText = await response.text();
            console.error('DEBUG: Erro da API:', errorText);

            try {
              const errorJson = JSON.parse(errorText);
              alert(`Erro ao salvar categoria: ${errorJson.message || JSON.stringify(errorJson)}`);
            } catch {
              alert(`Erro ao salvar categoria: ${errorText || response.statusText}`);
            }
          }
        } catch (error) {
          console.error('ERRO na requisição:', error);
          alert('Erro ao conectar com o servidor.');
        }
      });
      
    });