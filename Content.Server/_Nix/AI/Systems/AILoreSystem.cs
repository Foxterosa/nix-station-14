using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Robust.Shared.ContentPack;
using Robust.Shared.Log;
using Robust.Shared.Utility;

namespace Content.Server._Nix.AI.Systems;

/// <summary>
/// Sistema RAG de alta precisión para inyectar Lore canónico, guías del Guidebook y vademécum de química de SS14.
/// Carga 1,287 registros oficiales directamente en memoria RAM para búsqueda instantánea.
/// </summary>
public sealed class AILoreSystem : EntitySystem
{
    [Dependency] private readonly IResourceManager _resourceManager = default!;
    [Dependency] private readonly ILogManager _logManager = default!;

    private ISawmill _sawmill = default!;
    private readonly List<KnowledgeEntry> _knowledgeIndex = new();

    private static readonly HashSet<string> Stopwords = new(StringComparer.OrdinalIgnoreCase)
    {
        "jarrvis", "jarvis", "sparky", "como", "hago", "para", "que", "hacer", "con", "del", "los", "las",
        "una", "uno", "este", "esta", "estos", "estas", "por", "sobre", "donde", "cuando", "cual", "quien",
        "tengo", "hay", "mas", "menos", "aqui", "alli", "hola", "chau", "dime", "sabes", "favor"
    };

    public override void Initialize()
    {
        base.Initialize();
        _sawmill = _logManager.GetSawmill("ai_lore");
        LoadLoreDatabase();
    }

    private void LoadLoreDatabase()
    {
        _knowledgeIndex.Clear();
        try
        {
            var resPath = new ResPath("/_Nix/Lore/knowledge_index.json");
            if (_resourceManager.TryContentFileRead(resPath, out var stream))
            {
                using var reader = new StreamReader(stream, Encoding.UTF8);
                var json = reader.ReadToEnd();
                var list = JsonSerializer.Deserialize<List<KnowledgeEntry>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (list != null && list.Count > 0)
                {
                    _knowledgeIndex.AddRange(list);
                    _sawmill.Info($"[AILore] Cargados {_knowledgeIndex.Count} registros de conocimiento y recetas oficiales de SS14.");
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            _sawmill.Error($"[AILore] Error cargando knowledge_index.json: {ex}");
        }

        _sawmill.Warning("[AILore] No se pudo cargar knowledge_index.json del ContentPack.");
    }

    /// <summary>
    /// Busca y extrae el Lore y recetas canónicas relevantes evaluando todos los 1,287 registros del juego.
    /// </summary>
    public string GetRelevantLore(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || _knowledgeIndex.Count == 0)
            return string.Empty;

        var qLower = query.ToLowerInvariant();
        // Normalización de acentos de especie (Vulpkanin rr, ss, etc.)
        var qNorm = Regex.Replace(qLower, @"(.)\1+", "$1");
        
        var words = Regex.Matches(qNorm, @"\b[a-záéíóúñ]{3,}\b")
            .Select(m => m.Value)
            .Where(w => !Stopwords.Contains(w))
            .ToList();

        var scored = new List<(int score, KnowledgeEntry entry)>();

        foreach (var entry in _knowledgeIndex)
        {
            var score = 0;
            var tLow = entry.Title.ToLowerInvariant();
            var cLow = entry.Content.ToLowerInvariant();

            foreach (var w in words)
            {
                if (tLow.Contains(w))
                    score += 15;
                else if (cLow.Contains(w))
                    score += 3;
            }

            // Triggers específicos de alta relevancia canónica de SS14
            if (qNorm.Contains("dexalin") && tLow.Contains("dexalin")) score += 40;
            if (qNorm.Contains("inaprov") && tLow.Contains("inaprov")) score += 40;
            if (qNorm.Contains("bicarid") && tLow.Contains("bicarid")) score += 40;
            if (qNorm.Contains("kelotan") && tLow.Contains("kelotan")) score += 40;
            if (qNorm.Contains("dermal") && tLow.Contains("dermal")) score += 40;
            if (qNorm.Contains("dyloven") && tLow.Contains("dyloven")) score += 40;
            if (qNorm.Contains("tricord") && tLow.Contains("tricord")) score += 40;
            if (qNorm.Contains("tranexam") && tLow.Contains("tranexam")) score += 40;
            if (qNorm.Contains("synapt") && tLow.Contains("synapt")) score += 40;
            if (qNorm.Contains("cryox") && tLow.Contains("cryox")) score += 40;

            if (qNorm.Contains("supermat") && (tLow.Contains("supermat") || cLow.Contains("supermatter"))) score += 35;
            if ((qNorm.Contains("sindicat") || qNorm.Contains("sindical") || qNorm.Contains("traidor")) &&
                (tLow.Contains("syndicate") || tLow.Contains("sindicato") || cLow.Contains("sindicato"))) score += 35;
            
            if (qNorm.Contains("quemadur") && (tLow.Contains("dermaline") || tLow.Contains("kelotane"))) score += 25;
            if ((qNorm.Contains("golpe") || qNorm.Contains("corte") || qNorm.Contains("bruto")) && tLow.Contains("bicaridine")) score += 25;
            if ((qNorm.Contains("veneno") || qNorm.Contains("toxina")) && tLow.Contains("dylovene")) score += 25;
            if ((qNorm.Contains("aire") || qNorm.Contains("oxigeno") || qNorm.Contains("asfixia")) && (tLow.Contains("dexalin") || tLow.Contains("inaprovaline"))) score += 25;

            if (score > 0)
                scored.Add((score, entry));
        }

        scored.Sort((a, b) => b.score.CompareTo(a.score));

        var sb = new StringBuilder();
        var count = 0;
        foreach (var item in scored)
        {
            sb.AppendLine(item.entry.Content);
            sb.AppendLine();
            count++;
            if (count >= 3)
                break;
        }

        return sb.ToString().Trim();
    }

    /// <summary>
    /// Determina si una consulta requiere una búsqueda profunda en bases de datos que amerite un acuse de recibo.
    /// </summary>
    public bool RequiresDeepSearch(string query)
    {
        var qNorm = Regex.Replace(query.ToLowerInvariant(), @"(.)\1+", "$1");
        return qNorm.Contains("ley") || qNorm.Contains("supermat") ||
               qNorm.Contains("sindicat") || qNorm.Contains("protocolo") ||
               qNorm.Contains("quimic") || qNorm.Contains("tarea") ||
               qNorm.Contains("procedimiento") || qNorm.Contains("guia") ||
               qNorm.Contains("como hago") || qNorm.Contains("como creo") ||
               qNorm.Contains("como mezclo") || qNorm.Contains("dexalin") ||
               qNorm.Contains("inaprov") || qNorm.Contains("bicarid") ||
               qNorm.Contains("dermal") || qNorm.Contains("dyloven") ||
               qNorm.Contains("tricord") || qNorm.Contains("significa") || qNorm.Contains("sec");
    }
}

public sealed class KnowledgeEntry
{
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
