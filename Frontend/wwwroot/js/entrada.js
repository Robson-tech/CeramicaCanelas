// LOG 1: Confirma que o arquivo de script foi carregado e está sendo executado.
console.log('Script js/product-entry.js EXECUTANDO.');

/**
 * Função principal que inicializa o formulário de entrada de produto.
 */
function initializeProductEntryForm(form) {
    if (!form) {
        console.error('FALHA CRÍTICA: Elemento <form id="productForm"> não encontrado.');
        return;
    }

    console.log('🚀 Inicializando formulário de entrada de produto...');

    form.addEventListener('submit', (event) => {
        event.preventDefault(); // Impede o recarregamento da página
        console.log('Iniciando processamento dos dados de entrada...');
        processProductEntryData(form);
    });

    console.log('✅ Event listener do formulário configurado com sucesso!');
}

/**
 * Prepara os dados do formulário para envio.
 */
async function processProductEntryData(form) {
    console.log('🔍 Capturando e validando dados do formulário...');

    const productId = parseInt(form.productId.value.trim(), 10);
    const quantity = parseInt(form.quantity.value.trim(), 10);
    const unitPrice = parseFloat(form.unitPrice.value.trim());

    // Validações básicas
    if (isNaN(productId) || productId <= 0) {
        alert('Por favor, informe um ID de Produto válido (número inteiro).');
        return;
    }

    if (isNaN(quantity) || quantity <= 0) {
        alert('Por favor, informe uma quantidade válida.');
        return;
    }

    if (isNaN(unitPrice) || unitPrice < 0) {
        alert('Por favor, informe um preço unitário válido.');
        return;
    }

    // Monta o objeto JSON conforme a API espera
    const payload = {
        productId: productId,   // int32
        quantity: quantity,     // int32
        unitPrice: unitPrice    // float
    };

    console.log('✅ Dados prontos para envio:', payload);
    await sendProductEntryData(payload, form);
}

/**
 * Envia os dados da entrada de produto para a API.
 */
async function sendProductEntryData(payload, form) {
    console.log('📡 Preparando dados da entrada para envio...');

    const accessToken = localStorage.getItem('accessToken');
    if (!accessToken) {
        alert('Você não está autenticado. Faça o login novamente.');
        return;
    }

    try {
        const response = await fetch('http://localhost:5087/api/products-entry', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${accessToken}`
            },
            body: JSON.stringify(payload)
        });

        if (response.status === 401) {
            alert('Sessão expirada. Faça login novamente.');
            return;
        }

        if (response.ok) {
            console.log('✅ Entrada de produto registrada com sucesso!');
            alert('Entrada registrada com sucesso!');
            form.reset();
        } else {
            const errorData = await response.json().catch(() => ({}));
            const errorMessage = errorData.message || 'Erro ao registrar entrada. Verifique os dados.';
            console.error('❌ Erro da API:', errorMessage);
            alert(`Erro: ${errorMessage}`);
        }
    } catch (error) {
        console.error('❌ Erro na requisição:', error);
        alert('Falha na comunicação com o servidor. Verifique se a API está rodando.');
    }
}

// --- EXECUÇÃO PRINCIPAL ---
const formElement = document.querySelector('#productForm');
initializeProductEntryForm(formElement);
