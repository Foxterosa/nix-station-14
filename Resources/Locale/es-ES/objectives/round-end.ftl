objectives-round-end-result =
    { $count ->
        [one] Hubo una { $agent }.
        [few] Era { $count } { $agent }.
       *[other] Era { $count } { $agent }.
    }
objectives-round-end-result-in-custody = {$custody} de {$count} {MAKEPLURAL($agent)} estaban bajo custodia.

objectives-player-user-named = [color=White]{ $name }[/color] ([color=gray]{ $user }[/color])
objectives-player-named = [color=White]{ $name }[/color]
objectives-no-objectives = {$custody}{$title} era {$agent}.
objectives-with-objectives = {$custody}{$title} era {$agent} y tenía los siguientes objetivos:

objectives-objective-success = {$objective} | [color=green]¡Éxito![/color] ({TOSTRING($progress, "P0")})
objectives-objective-partial-success = {$objective} | [color=yellow]¡Éxito parcial![/color] ({TOSTRING($progress, "P0")})
objectives-objective-partial-failure = {$objective} | [color=orange]¡Fracaso parcial![/color] ({TOSTRING($progress, "P0")})
objectives-objective-fail = {$objective} | [color=red]¡Fracaso![/color] ({TOSTRING($progress, "P0")})

objectives-in-custody = [bold][color=red]| BAJO CUSTODIA | [/color][/bold]
