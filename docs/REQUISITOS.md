# Requisitos do CBTPRDM — Trabalho Prático 03

Dupla: **Matheus Correia França** e **Davi Leite Coelho — CB303142X**.

| Requisito do enunciado | Implementação |
| --- | --- |
| Projeto .NET MAUI | `src/Rastro.App/Rastro.App.csproj` |
| Interface em XAML | `Views/HomePage.xaml`, `Views/ResultsPage.xaml` e `Controls/WorkspaceView.xaml` |
| Entrada de código | Entry vinculada a `TrackingViewModel.Code` e `TrackCommand` |
| Tela de resultados | `ResultsPage`, navegação por `ITrackingNavigation` |
| Status atual | Badge, título, descrição e progresso de quatro etapas |
| Data de envio e entrega prevista | `Package.ShippedAt`, `ExpectedDelivery` e campos visíveis nos detalhes |
| Localização atual | `CurrentLocation`, último evento e mapa identificado como ilustrativo |
| Histórico de eventos | `Package.Events`, lista XAML com data, horário, descrição e cidade |
| Modelo de pacote | `Rastro.Core/Models/Package.cs` |
| ViewModel de rastreamento | `Rastro.Core/ViewModels/TrackingViewModel.cs` |
| Separação MVVM | Núcleo sem dependência de MAUI; bindings e comandos; serviços injetados |
| Busca simulada permitida | `DemoTrackingService`, quatro estados, latência e código desconhecido |
| Navegação intuitiva | Botão voltar, navegação lateral, cartões e ajuda em telas pequenas |
| Nome da dupla em comentários | `Package.cs` e `TrackingViewModel.cs` |
| Código no GitHub | https://github.com/matheuscorreiafranca/CBTPRDM_TP03 |
| Vídeo ou GIF | Evidências em `docs/`, roteiro em `ENTREGA.md` |

## Validação

Os testes automatizados verificam os quatro estados, consistência de datas, formato e normalização do código, busca vazia/inválida/desconhecida, navegação, histórico sem duplicados, filtros, contadores, salvar/remover, lista vazia, atualização, cópia, exceção de serviço, consultas concorrentes, reentrada de comando, falha de disco e recuperação de JSON corrompido.

A compilação Windows foi concluída sem erros ou avisos. Os **23 testes automatizados passaram**. A execução local foi validada com automação da interface para busca vazia, consulta, tela de detalhes, atualização, cópia, histórico e dimensionamento compacto; as capturas e o GIF estão nesta pasta. Android, iOS e Mac Catalyst dependem dos SDKs e dispositivos de cada plataforma e precisam de validação específica antes de distribuição. Não há integração de rastreamento real.
