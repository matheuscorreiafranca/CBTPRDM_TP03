using Rastro.Core.Models;

namespace Rastro.Core.Services;

/// <summary>Fonte fictícia e determinística por sessão. Não consulta transportadoras.</summary>
public sealed class DemoTrackingService : ITrackingService
{
    private readonly TimeSpan _latency;
    public IReadOnlyList<Package> DemoPackages { get; }

    public DemoTrackingService(TimeProvider? clock = null, TimeSpan? latency = null)
    {
        _latency = latency ?? TimeSpan.FromMilliseconds(650);
        var now = (clock ?? TimeProvider.System).GetUtcNow().ToOffset(TimeSpan.FromHours(-3));
        var morning = new DateTimeOffset(now.Year, now.Month, now.Day, 8, 30, 0, now.Offset);
        // Os horários dos eventos são sempre anteriores ao instante da consulta.
        var latest = morning < now ? morning : now.AddMinutes(-5);
        DemoPackages = [
            Create("BR123456789SP", "Um novo som para seus dias", "Eletrônicos", PackageStatus.InTransit,
                "São Paulo, SP", "Curitiba, PR", "Cubatão, SP", "Estúdio do Som", "0,45 kg", latest, 2),
            Create("BR987654321SP", "Sua próxima leitura", "Livros & papelaria", PackageStatus.OutForDelivery,
                "Cubatão, SP", "São Paulo, SP", "Cubatão, SP", "Livraria Capítulo", "0,80 kg", latest.AddMinutes(-12), 0),
            Create("BR456789123SP", "Detalhes para sua casa", "Casa & decoração", PackageStatus.Delivered,
                "Cubatão, SP", "Campinas, SP", "Cubatão, SP", "Casa Aurora", "1,20 kg", latest.AddDays(-1), -1),
            Create("BR111222333SP", "Pequenas grandes descobertas", "Acessórios", PackageStatus.Posted,
                "Santos, SP", "Santos, SP", "Cubatão, SP", "Ateliê Costa", "0,25 kg", latest, 4)
        ];
    }

    public async Task<Package?> FindAsync(string code, CancellationToken cancellationToken = default)
    {
        await Task.Delay(_latency, cancellationToken);
        return DemoPackages.FirstOrDefault(p => p.Id == TrackingCode.Normalize(code));
    }

    private static Package Create(string id, string name, string category, PackageStatus status,
        string location, string origin, string destination, string sender, string weight,
        DateTimeOffset latest, int deliveryOffset)
    {
        var events = new List<TrackingEvent>
        {
            new(latest.AddDays(-3), "Encomenda postada", "O remetente entregou o pacote à transportadora.", origin)
        };
        if (status >= PackageStatus.InTransit)
        {
            events.Add(new(latest.AddDays(-2), "Viagem iniciada", "A encomenda saiu da unidade de origem.", origin));
            events.Add(new(latest.AddHours(-3), "Chegou à unidade de distribuição", "Estamos preparando a próxima etapa da viagem.", location));
        }
        if (status == PackageStatus.InTransit)
            events.Add(new(latest, "Encomenda em trânsito", "Em transferência para a unidade que atende seu endereço.", location));
        if (status >= PackageStatus.OutForDelivery)
            events.Add(new(status == PackageStatus.Delivered ? latest.AddHours(-2) : latest,
                "Saiu para entrega", "A encomenda está com o entregador. Aguarde no endereço de destino.", destination));
        if (status == PackageStatus.Delivered)
            events.Add(new(latest, "Entrega realizada", "Encomenda recebida no endereço de destino.", destination));
        return new(id, name, category, status, events[0].Date,
            status == PackageStatus.Delivered ? latest : latest.AddDays(deliveryOffset),
            location, origin, destination, sender, "Matheus C. França", weight,
            events.OrderByDescending(e => e.Date).ToArray());
    }
}
