using System.Globalization;

namespace Rastro.Core.Models;

public enum PackageStatus { Posted, InTransit, OutForDelivery, Delivered }

public sealed record TrackingEvent(DateTimeOffset Date, string Title, string Description, string Location)
{
    public string Day => Date.ToString("dd MMM", CultureInfo.GetCultureInfo("pt-BR")).Replace(".", "");
    public string Time => Date.ToString("HH:mm");
}

// CBTPRDM — Trabalho Prático 03. Integrante: Matheus Correia França.
// Integrante: Davi Leite Coelho — CB303142X.
public sealed record Package(
    string Id, string Name, string Category, PackageStatus Status,
    DateTimeOffset ShippedAt, DateTimeOffset ExpectedDelivery,
    string CurrentLocation, string Origin, string Destination,
    string Sender, string Recipient, string Weight,
    IReadOnlyList<TrackingEvent> Events)
{
    private static readonly CultureInfo Portuguese = CultureInfo.GetCultureInfo("pt-BR");
    public string StatusLabel => Status switch
    {
        PackageStatus.Posted => "Postado",
        PackageStatus.InTransit => "Em trânsito",
        PackageStatus.OutForDelivery => "Saiu para entrega",
        _ => "Entregue"
    };
    public string StatusColor => Status switch
    {
        PackageStatus.Delivered => "#207A5A",
        PackageStatus.OutForDelivery => "#B35B17",
        _ => "#456783"
    };
    public string StatusBackground => Status switch
    {
        PackageStatus.Delivered => "#E8F5EE",
        PackageStatus.OutForDelivery => "#FFF0DC",
        _ => "#EDF3F8"
    };
    public string DeliveryLabel => Status == PackageStatus.Delivered ? "Entregue em" : "Previsão de entrega";
    public string DeliveryDate => ExpectedDelivery.ToString("dd 'de' MMMM", Portuguese);
    public string ShortDeliveryDate => ExpectedDelivery.ToString("dd MMM", Portuguese).Replace(".", "");
    public string ShippedDate => ShippedAt.ToString("dd/MM/yyyy", Portuguese);
    public string ExpectedDate => ExpectedDelivery.ToString("dd/MM/yyyy", Portuguese);
    public double Progress => ((int)Status + 1) / 4d;
    public string Headline => Status switch
    {
        PackageStatus.Posted => "O primeiro passo foi dado.",
        PackageStatus.InTransit => "Sua encomenda está a caminho.",
        PackageStatus.OutForDelivery => "Está quase nas suas mãos.",
        _ => "Uma boa notícia: chegou!"
    };
    public string StatusDescription => Status switch
    {
        PackageStatus.Posted => "Recebemos sua encomenda. Logo ela seguirá viagem até você.",
        PackageStatus.InTransit => "Tudo certo por aqui. Sua encomenda está seguindo para a próxima unidade.",
        PackageStatus.OutForDelivery => "Nossa última parada é você. A encomenda saiu para o endereço de destino.",
        _ => "Entrega concluída no endereço de destino. Obrigado por acompanhar com a gente."
    };
    public bool IsInTransit => Status >= PackageStatus.InTransit;
    public bool IsOutForDelivery => Status >= PackageStatus.OutForDelivery;
    public bool IsDelivered => Status == PackageStatus.Delivered;
}
