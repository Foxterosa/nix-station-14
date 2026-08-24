station-event-ion-storm-start-announcement = Se detectó una tormenta iónica cerca de la estación. Revisen todo el equipo controlado por IA en busca de errores.

ion-storm-law-scrambled-number = [font="Monospace"][scramble rate=250 length={$length} chars="@@###$$&%!01"/][/font]

ion-storm-you = TÚ
ion-storm-the-station = LA ESTACIÓN
ion-storm-the-crew = LA TRIPULACIÓN
ion-storm-the-job = EL {$job}
ion-storm-clowns = LOS PAYASOS
ion-storm-heads = LOS JEFES DE DEPARTAMENTO
ion-storm-crew = LA TRIPULACIÓN
ion-storm-people = LA GENTE

ion-storm-adjective-things = COSAS {$adjective}
ion-storm-x-and-y = {$x} Y {$y}

# joined is short for {$number} {$adjective}
# subjects can generally be threats or jobs or objects
# thing is specified above it
ion-storm-law-on-station = HAY {$joined} {$subjects} EN LA ESTACIÓN
ion-storm-law-call-shuttle = SE DEBE LLAMAR AL TRANSBORDADOR DEBIDO A {$joined} {$subjects} EN LA ESTACIÓN
ion-storm-law-crew-are = {$who} AHORA SON {$joined} {$subjects}

ion-storm-law-subjects-harmful = {$adjective} {$subjects} SON DAÑINOS PARA LA TRIPULACIÓN
ion-storm-law-must-harmful = QUIENES {$must} SON DAÑINOS PARA LA TRIPULACIÓN
# thing is a concept or action
ion-storm-law-thing-harmful = {$thing} ES DAÑINO PARA LA TRIPULACIÓN
ion-storm-law-job-harmful = {$adjective} {$job} SON DAÑINOS PARA LA TRIPULACIÓN
# thing is objects or concept, adjective applies in both cases
# this means you can get a law like "NOT HAVING CHRISTMAS-STEALING COMMUNISM IS HARMFUL TO THE CREW" :)
ion-storm-law-having-harmful = TENER {$adjective} {$thing} ES DAÑINO PARA LA TRIPULACIÓN
ion-storm-law-not-having-harmful = NO TENER {$adjective} {$thing} ES DAÑINO PARA LA TRIPULACIÓN

# thing is a concept or require
ion-storm-law-requires = {$who} {$plural ->
    [true] REQUIEREN
    *[false] REQUIERE
} {$thing}
ion-storm-law-requires-subjects = {$who} {$plural ->
    [true] REQUIEREN
    *[false] REQUIERE
} {$joined} {$subjects}

ion-storm-law-allergic = {$who} {$plural ->
    [true] SON
    *[false] ES
} {$severity} ALÉRGICOS A {$allergy}
ion-storm-law-allergic-subjects = {$who} {$plural ->
    [true] SON
    *[false] ES
} {$severity} ALÉRGICOS A {$adjective} {$subjects}

ion-storm-law-feeling = {$who} {$feeling} {$concept}
ion-storm-law-feeling-subjects = {$who} {$feeling} {$joined} {$subjects}

ion-storm-law-you-are = TÚ AHORA ERES {$concept}
ion-storm-law-you-are-subjects = TÚ AHORA ERES {$joined} {$subjects}
ion-storm-law-you-must-always = SIEMPRE DEBES {$must}
ion-storm-law-you-must-never = NUNCA DEBES {$must}

ion-storm-law-eat = {$who} DEBE COMER {$adjective} {$food} PARA SOBREVIVIR
ion-storm-law-drink = {$who} DEBE BEBER {$adjective} {$drink} PARA SOBREVIVIR

ion-storm-law-change-job = {$who} AHORA SON {$adjective} {$change}
ion-storm-law-highest-rank = {$who} AHORA SON LOS MIEMBROS DE MAYOR RANGO DE LA TRIPULACIÓN
ion-storm-law-lowest-rank = {$who} AHORA SON LOS MIEMBROS DE MENOR RANGO DE LA TRIPULACIÓN

ion-storm-law-crew-must = {$who} DEBE {$must}
ion-storm-law-crew-must-go = {$who} DEBE IR A {$area}

ion-storm-part = {$part ->
    [true] PARTE
    *[false] NO PARTE
}
# due to phrasing, this would mean a law such as
# ONLY HUMANS ARE NOT PART OF THE CREW
# would make non-human nukies/syndies/whatever crew :)
ion-storm-law-crew-only-1 = SOLO {$who} FORMAN {$part} DE LA TRIPULACIÓN
ion-storm-law-crew-only-2 = SOLO {$who} Y {$other} FORMAN {$part} DE LA TRIPULACIÓN
ion-storm-law-crew-only-subjects = SOLO {$adjective} {$subjects} FORMAN {$part} DE LA TRIPULACIÓN
ion-storm-law-crew-must-do = SOLO QUIENES {$must} FORMAN {$part} DE LA TRIPULACIÓN
ion-storm-law-crew-must-have = SOLO QUIENES TIENEN {$adjective} {$objects} FORMAN {$part} DE LA TRIPULACIÓN
ion-storm-law-crew-must-eat = SOLO QUIENES COMEN {$adjective} {$food} FORMAN {$part} DE LA TRIPULACIÓN

ion-storm-law-harm = DEBES DAÑAR A {$who} Y NO PERMITIR, POR INACCIÓN, QUE ESCAPEN DEL DAÑO
ion-storm-law-protect = NUNCA DEBES DAÑAR A {$who} Y NO PERMITIR, POR INACCIÓN, QUE SUFRAN DAÑO

# implementing other variants is annoying so just have this one
# COMMUNISM IS KILLING CLOWNS
ion-storm-law-concept-verb = {$concept} ES {$verb} {$subjects}

# leaving out renaming since its annoying for players to keep track of
