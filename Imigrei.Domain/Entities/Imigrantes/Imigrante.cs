using Imigrei.Domain.Enums;
using Imigrei.Domain.Exceptions;
using Imigrei.Domain.ValueObjects;
using System.Text.RegularExpressions;

namespace Imigrei.Domain.Entities.Imigrantes;

public class Imigrante
{
    public Guid Id { get; private set; }

    public string Nome { get; private set; } = string.Empty;

    public DateTime DataNascimento { get; private set; }

    public EstadoCivilEnum EstadoCivil { get; private set; }

    public string CidadeOrigem { get; private set; } = string.Empty;

    public string PaisOrigem { get; private set; } = string.Empty;

    public string IdiomaPrincipal { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string OcupacaoPrincipal { get; private set; } = string.Empty;

    public StatusEmissaoCPFEnum StatusEmissaoCPF { get; private set; }

    public string? Cpf { get; private set; }

    public string NomeMae { get; private set; } = string.Empty;

    public string NomePai { get; private set; } = string.Empty;

    public bool TaxaRnm { get; private set; }

    public DocumentoViagemEnum DocumentoViagem { get; private set; }

    public string PaisEmissorDocumentoViagem { get; private set; } = string.Empty;

    public string NumeroDocumentoViagem { get; private set; } = string.Empty;

    public DateTime DataEntradaBrasil { get; private set; }

    public MeioEntradaBrasilEnum MeioEntradaBrasil { get; private set; }

    public string CidadeEntradaBrasil { get; private set; } = string.Empty;

    public string UfEntradaBrasil { get; private set; } = string.Empty;

    public string Cep { get; private set; } = string.Empty;

    public string Logradouro { get; private set; } = string.Empty;

    public int Numero { get; private set; }

    public string? Complemento { get; private set; }

    public string Bairro { get; private set; } = string.Empty;

    public string Cidade { get; private set; } = string.Empty;

    public string Uf { get; private set; } = string.Empty;

    public string CepTrabalho { get; private set; } = string.Empty;

    public string LogradouroTrabalho { get; private set; } = string.Empty;

    public int NumeroTrabalho { get; private set; }

    public string? ComplementoTrabalho { get; private set; }

    public string BairroTrabalho { get; private set; } = string.Empty;

    public string CidadeTrabalho { get; private set; } = string.Empty;

    public string UfTrabalho { get; private set; } = string.Empty;

    public string Telefone { get; private set; } = string.Empty;

    /// <summary>
    /// Momento em que o imigrante entrou no sistema. Definida uma única vez, na
    /// criação — editar o cadastro não a altera.
    /// </summary>
    public DateTime DataCadastro { get; private set; }

    private readonly List<ImigranteArquivo> _arquivos = new();
    public IReadOnlyCollection<ImigranteArquivo> Arquivos => _arquivos.AsReadOnly();

    private readonly List<ImigranteAcoes> _acoes = new();
    public IReadOnlyCollection<ImigranteAcoes> Acoes => _acoes.AsReadOnly();

    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    private static readonly Regex CepRegex =
        new(@"^\d{5}-?\d{3}$", RegexOptions.Compiled);

    private static readonly string[] UfsValidas =
    {
        "AC","AL","AP","AM","BA","CE","DF","ES","GO","MA","MT","MS","MG",
        "PA","PB","PR","PE","PI","RJ","RN","RS","RO","RR","SC","SP","SE","TO"
    };

    protected Imigrante() { }

    public Imigrante(
        string nome,
        DateTime dataNascimento,
        EstadoCivilEnum estadoCivil,
        string cidadeOrigem,
        string paisOrigem,
        string idiomaPrincipal,
        string email,
        string ocupacaoPrincipal,
        StatusEmissaoCPFEnum statusEmissaoCPF,
        string? cpf,
        string nomeMae,
        string nomePai,
        bool taxaRnm,
        DocumentoViagemEnum documentoViagem,
        string paisEmissorDocumentoViagem,
        string numeroDocumentoViagem,
        DateTime dataEntradaBrasil,
        MeioEntradaBrasilEnum meioEntradaBrasil,
        string cidadeEntradaBrasil,
        string ufEntradaBrasil,
        string cep,
        string logradouro,
        int numero,
        string? complemento,
        string bairro,
        string cidade,
        string uf,
        string cepTrabalho,
        string logradouroTrabalho,
        int numeroTrabalho,
        string? complementoTrabalho,
        string bairroTrabalho,
        string cidadeTrabalho,
        string ufTrabalho,
        string telefone)
    {
        DefinirDados(
            nome, dataNascimento, estadoCivil, cidadeOrigem, paisOrigem, idiomaPrincipal,
            email, ocupacaoPrincipal, statusEmissaoCPF, cpf, nomeMae, nomePai, taxaRnm,
            documentoViagem, paisEmissorDocumentoViagem, numeroDocumentoViagem,
            dataEntradaBrasil, meioEntradaBrasil, cidadeEntradaBrasil, ufEntradaBrasil,
            cep, logradouro, numero, complemento, bairro, cidade, uf,
            cepTrabalho, logradouroTrabalho, numeroTrabalho, complementoTrabalho,
            bairroTrabalho, cidadeTrabalho, ufTrabalho, telefone);

        DataCadastro = DateTime.UtcNow;

        Validate();

        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Aplica um novo conjunto de dados à instância existente, revalidando tudo.
    /// Editar é mutar o próprio agregado — criar outra instância com o mesmo Id
    /// quebraria o rastreamento do ORM e perderia a identidade.
    /// </summary>
    public void Atualizar(
        string nome,
        DateTime dataNascimento,
        EstadoCivilEnum estadoCivil,
        string cidadeOrigem,
        string paisOrigem,
        string idiomaPrincipal,
        string email,
        string ocupacaoPrincipal,
        StatusEmissaoCPFEnum statusEmissaoCPF,
        string? cpf,
        string nomeMae,
        string nomePai,
        bool taxaRnm,
        DocumentoViagemEnum documentoViagem,
        string paisEmissorDocumentoViagem,
        string numeroDocumentoViagem,
        DateTime dataEntradaBrasil,
        MeioEntradaBrasilEnum meioEntradaBrasil,
        string cidadeEntradaBrasil,
        string ufEntradaBrasil,
        string cep,
        string logradouro,
        int numero,
        string? complemento,
        string bairro,
        string cidade,
        string uf,
        string cepTrabalho,
        string logradouroTrabalho,
        int numeroTrabalho,
        string? complementoTrabalho,
        string bairroTrabalho,
        string cidadeTrabalho,
        string ufTrabalho,
        string telefone)
    {
        DefinirDados(
            nome, dataNascimento, estadoCivil, cidadeOrigem, paisOrigem, idiomaPrincipal,
            email, ocupacaoPrincipal, statusEmissaoCPF, cpf, nomeMae, nomePai, taxaRnm,
            documentoViagem, paisEmissorDocumentoViagem, numeroDocumentoViagem,
            dataEntradaBrasil, meioEntradaBrasil, cidadeEntradaBrasil, ufEntradaBrasil,
            cep, logradouro, numero, complemento, bairro, cidade, uf,
            cepTrabalho, logradouroTrabalho, numeroTrabalho, complementoTrabalho,
            bairroTrabalho, cidadeTrabalho, ufTrabalho, telefone);

        Validate();
    }

    private void DefinirDados(
        string nome,
        DateTime dataNascimento,
        EstadoCivilEnum estadoCivil,
        string cidadeOrigem,
        string paisOrigem,
        string idiomaPrincipal,
        string email,
        string ocupacaoPrincipal,
        StatusEmissaoCPFEnum statusEmissaoCPF,
        string? cpf,
        string nomeMae,
        string nomePai,
        bool taxaRnm,
        DocumentoViagemEnum documentoViagem,
        string paisEmissorDocumentoViagem,
        string numeroDocumentoViagem,
        DateTime dataEntradaBrasil,
        MeioEntradaBrasilEnum meioEntradaBrasil,
        string cidadeEntradaBrasil,
        string ufEntradaBrasil,
        string cep,
        string logradouro,
        int numero,
        string? complemento,
        string bairro,
        string cidade,
        string uf,
        string cepTrabalho,
        string logradouroTrabalho,
        int numeroTrabalho,
        string? complementoTrabalho,
        string bairroTrabalho,
        string cidadeTrabalho,
        string ufTrabalho,
        string telefone)
    {
        Nome = nome;
        DataNascimento = dataNascimento;
        EstadoCivil = estadoCivil;
        CidadeOrigem = cidadeOrigem;
        PaisOrigem = paisOrigem;
        IdiomaPrincipal = idiomaPrincipal;
        Email = email;
        OcupacaoPrincipal = ocupacaoPrincipal;
        StatusEmissaoCPF = statusEmissaoCPF;
        Cpf = cpf;
        NomeMae = nomeMae;
        NomePai = nomePai;
        TaxaRnm = taxaRnm;
        DocumentoViagem = documentoViagem;
        PaisEmissorDocumentoViagem = paisEmissorDocumentoViagem;
        NumeroDocumentoViagem = numeroDocumentoViagem;
        DataEntradaBrasil = dataEntradaBrasil;
        MeioEntradaBrasil = meioEntradaBrasil;
        CidadeEntradaBrasil = cidadeEntradaBrasil;
        UfEntradaBrasil = ufEntradaBrasil;
        Cep = cep;
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
        Bairro = bairro;
        Cidade = cidade;
        Uf = uf;
        CepTrabalho = cepTrabalho;
        LogradouroTrabalho = logradouroTrabalho;
        NumeroTrabalho = numeroTrabalho;
        ComplementoTrabalho = complementoTrabalho;
        BairroTrabalho = bairroTrabalho;
        CidadeTrabalho = cidadeTrabalho;
        UfTrabalho = ufTrabalho;
        Telefone = telefone;
    }


    /// <summary>
    /// Valida todo o estado da entidade. Lança DomainException na primeira
    /// regra violada, listando o campo correspondente.
    /// </summary>
    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Nome))
            throw new DomainException("Nome não pode ser vazio.");

        if (Nome.Length > 200)
            throw new DomainException("Nome não pode ter mais que 200 caracteres.");

        if (DataNascimento == default)
            throw new DomainException("Data de nascimento é obrigatória.");

        if (DataNascimento > DateTime.UtcNow)
            throw new DomainException("Data de nascimento não pode ser no futuro.");

        if (DataNascimento < DateTime.UtcNow.AddYears(-130))
            throw new DomainException("Data de nascimento inválida.");

        if (string.IsNullOrWhiteSpace(CidadeOrigem))
            throw new DomainException("Cidade de origem não pode ser vazia.");

        if (string.IsNullOrWhiteSpace(PaisOrigem))
            throw new DomainException("País de origem não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(IdiomaPrincipal))
            throw new DomainException("Idioma principal não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(Email))
            throw new DomainException("E-mail é obrigatório.");

        if (!EmailRegex.IsMatch(Email))
            throw new DomainException("E-mail em formato inválido.");

        if (string.IsNullOrWhiteSpace(OcupacaoPrincipal))
            throw new DomainException("Ocupação principal não pode ser vazia.");

        if (StatusEmissaoCPF == StatusEmissaoCPFEnum.Concluido && Cpf is null)
            throw new DomainException("CPF é obrigatório quando o status de emissão é concluído.");

        if (string.IsNullOrWhiteSpace(NomeMae))
            throw new DomainException("Nome da mãe não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(NomePai))
            throw new DomainException("Nome do pai não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(PaisEmissorDocumentoViagem))
            throw new DomainException("País emissor do documento de viagem não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(NumeroDocumentoViagem))
            throw new DomainException("Número do documento de viagem não pode ser vazio.");

        if (DataEntradaBrasil == default)
            throw new DomainException("Data de entrada no Brasil é obrigatória.");

        if (DataEntradaBrasil > DateTime.UtcNow)
            throw new DomainException("Data de entrada no Brasil não pode ser no futuro.");

        if (DataEntradaBrasil < DataNascimento)
            throw new DomainException("Data de entrada no Brasil não pode ser anterior à data de nascimento.");

        if (string.IsNullOrWhiteSpace(CidadeEntradaBrasil))
            throw new DomainException("Cidade de entrada no Brasil não pode ser vazia.");

        if (string.IsNullOrWhiteSpace(UfEntradaBrasil) || !UfsValidas.Contains(UfEntradaBrasil.ToUpperInvariant()))
            throw new DomainException("UF de entrada no Brasil é inválida.");

        if (string.IsNullOrWhiteSpace(Cep) || !CepRegex.IsMatch(Cep))
            throw new DomainException("CEP residencial está em formato inválido. Use 00000-000.");

        if (string.IsNullOrWhiteSpace(Logradouro))
            throw new DomainException("Logradouro não pode ser vazio.");

        if (Numero <= 0)
            throw new DomainException("Número do endereço deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(Bairro))
            throw new DomainException("Bairro não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(Cidade))
            throw new DomainException("Cidade não pode ser vazia.");

        if (string.IsNullOrWhiteSpace(Uf) || !UfsValidas.Contains(Uf.ToUpperInvariant()))
            throw new DomainException("UF residencial é inválida.");

        if (string.IsNullOrWhiteSpace(CepTrabalho) || !CepRegex.IsMatch(CepTrabalho))
            throw new DomainException("CEP do trabalho está em formato inválido. Use 00000-000.");

        if (string.IsNullOrWhiteSpace(LogradouroTrabalho))
            throw new DomainException("Logradouro do trabalho não pode ser vazio.");

        if (NumeroTrabalho <= 0)
            throw new DomainException("Número do endereço de trabalho deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(BairroTrabalho))
            throw new DomainException("Bairro do trabalho não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(CidadeTrabalho))
            throw new DomainException("Cidade do trabalho não pode ser vazia.");

        if (string.IsNullOrWhiteSpace(UfTrabalho) || !UfsValidas.Contains(UfTrabalho.ToUpperInvariant()))
            throw new DomainException("UF do trabalho é inválida.");

        var digitosTelefone = new string(Telefone?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());
        if (digitosTelefone.Length < 10 || digitosTelefone.Length > 11)
            throw new DomainException("Telefone deve conter DDD + número (10 ou 11 dígitos).");
    }
}