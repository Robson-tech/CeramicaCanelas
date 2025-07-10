document.addEventListener('DOMContentLoaded', () => {
  console.log('DOM completamente carregado. Iniciando script.');

  const categoryForm = document.querySelector('.category-form');

  if (categoryForm) {
    console.log('Formulário encontrado com a classe .category-form');
    categoryForm.addEventListener('submit', async (event) => {
      console.log('Evento de submit do formulário detectado.');
      event.preventDefault(); // Impede o comportamento padrão de envio do formulário

      // Cria um objeto FormData a partir do formulário
      const formData = new FormData();
      const categoryName = categoryForm.querySelector('input[name="category"]').value;
      const categoryDescription = categoryForm.querySelector('textarea[name="description"]').value;

      // Adiciona os campos ao FormData com os nomes que a API espera
      // (Certifique-se de que 'Name' e 'Description' são os nomes exatos esperados pela API)
      formData.append('Name', categoryName);
      formData.append('Description', categoryDescription);
      // Se houver um ImageUrl opcional, adicione-o também.
      // Por exemplo, se você tivesse um input de arquivo:
      // const imageFile = categoryForm.querySelector('input[name="imageUrl"]').files[0];
      // if (imageFile) {
      //     formData.append('ImageUrl', imageFile);
      // } else {
      //     formData.append('ImageUrl', ''); // Envia string vazia se não houver imagem
      // }
      // Para o seu caso, se ImageUrl é apenas uma string e não um arquivo, mantenha como string vazia se não tiver um campo específico
      formData.append('ImageUrl', ''); // Ou o valor de um campo de URL de imagem, se houver.

      console.log('Dados coletados do formulário para FormData:');
      // Para debug, você pode iterar sobre o FormData para ver os valores
      for (let pair of formData.entries()) {
        console.log(pair[0] + ': ' + pair[1]);
      }

      try {
        console.log('Enviando requisição para:', 'http://localhost:5087/api/categories');
        // Ao usar FormData, não defina Content-Type; o navegador faz isso automaticamente
        const response = await fetch('http://localhost:5087/api/categories', {
          method: 'POST',
          body: formData, // Envia o objeto FormData diretamente
        });

        console.log('Resposta da API recebida. Status:', response.status);

        if (response.ok) {
          console.log('Requisição bem-sucedida!');
          alert('Categoria salva com sucesso!');
          categoryForm.reset();
        } else {
          // Tente ler a resposta como texto se não for JSON, ou JSON se for.
          // Às vezes, erros de FormData vêm como texto puro.
          const errorText = await response.text();
          try {
              const errorData = JSON.parse(errorText);
              console.error('Erro na resposta da API (JSON):', errorData);
              alert(`Erro ao salvar categoria: ${errorData.message || JSON.stringify(errorData) || response.statusText}`);
          } catch (e) {
              console.error('Erro na resposta da API (Texto):', errorText);
              alert(`Erro ao salvar categoria: ${errorText || response.statusText}`);
          }
        }
      } catch (error) {
        console.error('Erro na requisição Fetch:', error);
        alert('Ocorreu um erro ao tentar conectar ao servidor.');
      }
    });
  } else {
    console.error('ERRO: Formulário com a classe ".category-form" NÃO encontrado!');
  }
});