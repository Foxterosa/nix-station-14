ammonia-smell = ¡Algo tiene un olor penetrante!
perishable-1 = [color=green]El cuerpo de { CAPITALIZE(OBJECT($target)) } aún parece fresco.[/color]
perishable-2 = [color=orangered]El cuerpo de { CAPITALIZE(OBJECT($target)) } parece relativamente fresco.[/color]
perishable-3 = [color=red]El cuerpo de { CAPITALIZE(OBJECT($target)) } ya no parece fresco.[/color]
perishable-1-nonmob = [color=green]{ CAPITALIZE(SUBJECT($target)) } aún parece fresco.[/color]
perishable-2-nonmob = [color=orangered]{ CAPITALIZE(SUBJECT($target)) } parece relativamente fresco.[/color]
perishable-3-nonmob = [color=red]{ CAPITALIZE(SUBJECT($target)) } ya no parece fresco.[/color]
rotting-rotting = [color=orange]¡El cuerpo de { CAPITALIZE(SUBJECT($target)) } { GENDER($target) ->
        [male] se está descomponiendo
        [female] se está descomponiendo
        [epicene] se está descomponiendo
       *[neuter] se está descomponiendo
    }![/color]
rotting-bloated = [color=orangered]¡El cuerpo de { CAPITALIZE(SUBJECT($target)) } { GENDER($target) ->
        [male] está hinchado
        [female] está hinchado
        [epicene] está hinchado
       *[neuter] está hinchado
    }![/color]
rotting-extremely-bloated = [color=red]¡El cuerpo de { CAPITALIZE(SUBJECT($target)) } { GENDER($target) ->
        [male] está extremadamente hinchado
        [female] está extremadamente hinchado
        [epicene] está extremadamente hinchado
       *[neuter] está extremadamente hinchado
    }![/color]
rotting-rotting-nonmob = [color=orange]¡{ CAPITALIZE(SUBJECT($target)) } se está descomponiendo![/color]
rotting-bloated-nonmob = [color=orangered]¡{ CAPITALIZE(SUBJECT($target)) } está hinchado![/color]
rotting-extremely-bloated-nonmob = [color=red]¡{ CAPITALIZE(SUBJECT($target)) } está extremadamente hinchado![/color]
