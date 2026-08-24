shared-solution-container-component-on-examine-main-text = Contiene [color={$color}]{$desc}[/color] { $chemCount ->
    [1] sustancia química.
   *[other] mezcla de sustancias químicas.
    }

examinable-solution-has-recognizable-chemicals = Reconoces {$recognizedString} en la solución.
examinable-solution-recognized = [color={$color}]{$chemical}[/color]

examinable-solution-on-examine-volume = La solución que contiene { $fillLevel ->
    [exact] está en [color=white]{$current}/{$max}u[/color].
   *[other] está [bold]{ -solution-vague-fill-level(fillLevel: $fillLevel) }[/bold].
}

examinable-solution-on-examine-volume-no-max = La solución que contiene { $fillLevel ->
    [exact] está en [color=white]{$current}u[/color].
   *[other] está [bold]{ -solution-vague-fill-level(fillLevel: $fillLevel) }[/bold].
}

examinable-solution-on-examine-volume-puddle = El charco { $fillLevel ->
    [exact] tiene [color=white]{$current}u[/color].
    [full] ¡es enorme y se desborda!
    [mostlyfull] ¡es enorme y se desborda!
    [halffull] es profundo y fluye.
    [halfempty] es muy profundo.
   *[mostlyempty] se está juntando.
    [empty] forma varios charquitos.
}

-solution-vague-fill-level =
    { $fillLevel ->
        [full] [color=white]Lleno[/color]
        [mostlyfull] [color=#DFDFDF]Casi lleno[/color]
        [halffull] [color=#C8C8C8]Medio lleno[/color]
        [halfempty] [color=#C8C8C8]Medio vacío[/color]
        [mostlyempty] [color=#A4A4A4]Casi vacío[/color]
       *[empty] [color=gray]Vacío[/color]
    }
