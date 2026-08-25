guidebook-reagent-effect-description =
    {$quantity ->
        [0] {""}
        *[other] Si hay al menos {$quantity}u de {$reagent},{" "}
    }{$chance ->
        [1] { $effect }
        *[other] Tiene un { NATURALPERCENT($chance, 2) } de probabilidad de { $effect }
    }{ $conditionCount ->
        [0] .
        *[other] {" "}cuando { $conditions }.
    }

guidebook-reagent-name = [bold][color={$color}]{CAPITALIZE($name)}[/color][/bold]
guidebook-reagent-recipes-header = Receta
guidebook-reagent-recipes-reagent-display = [bold]{$reagent}[/bold] \[{$ratio}\]
guidebook-reagent-sources-header = Fuentes
guidebook-reagent-sources-ent-wrapper = [bold]{$name}[/bold] \[1\]
guidebook-reagent-sources-gas-wrapper = [bold]{$name} (gas)[/bold] \[1\]
guidebook-reagent-effects-header = Efectos
guidebook-reagent-effects-metabolism-stage-rate = [bold]{$stage}[/bold] [color=gray]({$rate} unidades por segundo)[/color]
guidebook-reagent-effects-metabolite-item = {$reagent} a una tasa de { NATURALPERCENT($rate, 2) }
guidebook-reagent-effects-metabolites = Se metaboliza en { $items }.
guidebook-reagent-plant-metabolisms-header = Metabolismo vegetal
guidebook-reagent-plant-metabolisms-rate = [bold]Metabolismo vegetal[/bold] [color=gray](1 unidad cada 3 segundos como base)[/color]
guidebook-reagent-physical-description = [italic]Parece ser {$description}.[/italic]
guidebook-reagent-recipes-mix-info = {$minTemp ->
    [0] {$hasMax ->
            [true] {CAPITALIZE($verb)} por debajo de {NATURALFIXED($maxTemp, 2)}K
            *[false] {CAPITALIZE($verb)}
        }
    *[other] {CAPITALIZE($verb)} {$hasMax ->
            [true] entre {NATURALFIXED($minTemp, 2)}K y {NATURALFIXED($maxTemp, 2)}K
            *[false] por encima de {NATURALFIXED($minTemp, 2)}K
        }
}
