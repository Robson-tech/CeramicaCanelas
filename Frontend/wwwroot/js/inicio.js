console.log('Script js/home.js DEFINIDO.');

// =======================================================
// INICIALIZAÇÃO
// =======================================================
function initDynamicForm() {
    console.log('▶️ initDynamicForm() de home.js foi chamada.');
    fetchDashboardData();
}

// =======================================================
// LÓGICA DA DASHBOARD
// =======================================================

async function fetchDashboardData() {
    try {
        const accessToken = localStorage.getItem('accessToken');
        if (!accessToken) throw new Error("Não autenticado.");

        const response = await fetch(`${API_BASE_URL}/dashboard/primary`, {
            headers: { 'Authorization': `Bearer ${accessToken}` }
        });
        if (!response.ok) throw new Error(`Falha ao buscar dados (Status: ${response.status})`);

        const data = await response.json();
        
        updateDashboardCards(data);
        renderEntriesChart(data.entradasPorMes);

    } catch (error) {
        console.error("❌ Erro ao carregar dados da dashboard:", error);
        showErrorModal({ title: "Erro na Dashboard", detail: error.message });
    }
}

function updateDashboardCards(data) {
    // Formata o valor monetário
    const formattedSpending = (data.gastoTotalComprasMes || 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });

    // Atualiza o texto dos cards
    document.getElementById('total-products-value').textContent = data.totalProdutosCadastrados;
    document.getElementById('total-stock-value').textContent = data.totalProdutosEstoque;
    document.getElementById('monthly-spending-value').textContent = formattedSpending;
    document.getElementById('stock-alerts-value').textContent = data.alertasEstoqueMinimo;
}

function renderEntriesChart(monthlyEntries) {
    const ctx = document.getElementById('entradasChart');
    if (!ctx) return;

    const labels = ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'];

    new Chart(ctx, {
        type: 'bar', // ou 'line'
        data: {
            labels: labels,
            datasets: [{
                label: 'Entradas de Produtos',
                data: monthlyEntries,
                backgroundColor: 'rgba(253, 126, 20, 0.6)',
                borderColor: 'rgba(253, 126, 20, 1)',
                borderWidth: 1,
                borderRadius: 5
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true
                }
            },
            plugins: {
                legend: {
                    display: false
                }
            }
        }
    });
}