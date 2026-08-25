execution-verb-name = Ejecutar
execution-verb-message = Usa tu arma para ejecutar a alguien.

suicide-verb-name = Suicidarse
suicide-verb-message = Usa tu arma para suicidarte.

# All the below localisation strings have access to the following variables
# attacker (the person committing the execution)
# victim (the person being executed)
# weapon (the weapon used for the execution)

# STARLIGHT CONTROLLED
# God these need to move to their own file
execution-popup-melee-initial-internal = Preparas {THE($weapon)} contra la garganta de {THE($victim)}.
execution-popup-gun-initial-internal = Apuntas con la boca de {THE($weapon)} a la cabeza de {THE($victim)}.

execution-popup-melee-initial-external = { CAPITALIZE(THE($attacker)) } prepara {POSS-ADJ($attacker)} {$weapon} contra la garganta de {THE($victim)}.
execution-popup-gun-initial-external  = { CAPITALIZE(THE($attacker)) } apunta con la boca de {POSS-ADJ($attacker)} {$weapon} a la cabeza de {THE($victim)}.

execution-popup-melee-complete-internal = ¡Le cortas la garganta a {THE($victim)}!
execution-popup-gun-complete-internal = ¡Le disparas a {THE($victim)} en la cabeza!

execution-popup-melee-complete-external = ¡{ CAPITALIZE(THE($attacker)) } le corta la garganta a {THE($victim)}!
execution-popup-gun-complete-external = ¡{ CAPITALIZE(THE($attacker)) } le dispara a {THE($victim)} en la cabeza!

execution-popup-gun-clumsy-internal = ¡Fallas la cabeza de {THE($victim)} y te disparas en el pie!
execution-popup-gun-clumsy-external = ¡{ CAPITALIZE(THE($attacker)) } falla a {THE($victim)} y se dispara en el pie!

execution-popup-gun-empty = { CAPITALIZE(THE($weapon)) } hace clic.

execution-popup-self-melee-initial-internal = Preparas {THE($weapon)} contra tu propia garganta.
execution-popup-self-gun-initial-internal = Te metes la boca de {THE($weapon)} en la boca.

execution-popup-self-melee-initial-external = { CAPITALIZE(THE($attacker)) } prepara {POSS-ADJ($attacker)} {$weapon} contra su propia garganta.
execution-popup-self-gun-initial-external = { CAPITALIZE(THE($attacker)) } se mete la boca de {POSS-ADJ($attacker)} {$weapon} en la boca.

execution-popup-self-melee-complete-internal = ¡Te cortas la garganta!
execution-popup-self-gun-complete-internal = ¡Te estás disparando en la cabeza!

execution-popup-self-melee-complete-external = ¡{ CAPITALIZE(THE($attacker)) } se corta la garganta!
execution-popup-self-gun-complete-external = ¡{ CAPITALIZE(THE($attacker)) } se dispara en la cabeza!
# Starlight end
