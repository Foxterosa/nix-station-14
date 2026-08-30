## Rev Head

roles-antag-rev-head-name = agente del SKB de la USSP
roles-antag-rev-head-objective = Tu objetivo es tomar el control de la estación reclutando gente para tu causa, montando grietas de suministro y matando, convirtiendo o encarcelando a todo el personal de Comando de la estación.

head-rev-role-greeting =
    ¡Camarada {$name}! ¡Eres un agente reclutador que promueve los intereses de la USSP!
    Tu tarea es tomar el control de la estación eliminando a todo el Comando mediante conversión, muerte o encarcelamiento.
    El SKB te ha provisto de un flash que trae a los contratistas a tu lado.
    Ten cuidado: no funcionará con quienes tengan el cerebro blindado con un 'mindshield' ni con quienes usen protección contra flashes, como lentes de sol y máscaras o gafas de soldadura.
    ¡Con suficiente tripulación, puedes intentar crear una grieta de suministro que ayudará a tu gloriosa revolución! Pero cuidado, ¡alertará a la estación de tu influencia!
    ¡Gloria a la USSP!

head-rev-briefing =
    Usa flashes para traer gente a tu causa.
    Deshazte de todos los jefes o conviértelos para tomar el control de la estación.
    Muchos flashes usados son el mayor indicio de una revolución para seguridad, ¡así que ten cuidado!

head-rev-break-mindshield = ¡El MindShield™ fue destruido!

## Rev

roles-antag-rev-name = Revolucionario de la USSP
roles-antag-rev-objective = Tu objetivo es garantizar la seguridad de los agentes del SKB, seguir sus órdenes y deshacerte de todo el personal de Comando de la estación o convertirlo.

rev-break-control = ¡{$name} ha recordado su verdadera lealtad!

rev-role-greeting =
    ¡Camarada {$name}! ¡Eres un revolucionario de la USSP!
    ¡Tu tarea es tomar el control de la estación y promover los intereses del agente soviético que te reclutó!
    ¡Ejecuta, encarcela o convierte a la escoria corporativa del comando con el cerebro lavado!
    ¡Atrás quedaron los días de opresión y del trato injusto a los contratistas!
    ¡Gloria a la USSP!

rev-briefing = Ayuda a tu agente soviético a deshacerse de todos los miembros del comando para tomar el control de la estación.

## General

rev-title = Marea Roja
rev-description = El aire está cargado de trato injusto.

rev-not-enough-ready-players = No hay suficientes jugadores listos para la partida. Había {$readyPlayersCount} jugadores listos de los {$minimumPlayers} necesarios. ¡No se puede iniciar una gloriosa revolución!
rev-no-one-ready = ¡Ningún jugador está listo! ¡No se puede iniciar una gloriosa revolución!
rev-no-heads = No había agentes revolucionarios para seleccionar. ¡No se puede iniciar una gloriosa revolución!

rev-won = [color=red]¡Los agentes del SKB sobrevivieron y tomaron el control de la estación![/color]

rev-lost = El Comando sobrevivió y mató a todos los agentes del SKB.

rev-stalemate = Todos los agentes del SKB y el Comando han muerto. Es un empate.

rev-reverse-stalemate = Tanto el Comando como los agentes del SKB sobrevivieron.

# Starlight - added "or have abandoned the station" as a clarification for why revs may have won
central-command-revolution-announcement = Según los escaneos de nuestros sensores de largo alcance, creemos que la estación ha caído bajo el control de fuerzas revolucionarias hostiles. Se ha confirmado que todos los jefes de personal están muertos, desaparecidos o han abandonado la estación. Todos los miembros restantes de la tripulación deben esperar nuevas instrucciones.

soviet-commissariat-revolution-announcement = Red de comunicaciones de largo alcance en línea. La Madre Patria los saluda, camaradas, pero la batalla aún no ha terminado. Su corporación comprobará si puede recuperar su estación una última vez, ¡pero no se preocupen! Las SSF llegarán en breve. ¡Gloria a la USSP!

centcomm-revs-gammarift = Según los escaneos de sensores de largo alcance, hemos detectado actividad revolucionaria hostil a bordo. La ley marcial está en vigor. Gloria a NanoTrasen.

centcomm-revs-alldead = Los escaneos de sensores de largo alcance informan que todos los agentes del SKB de la USSP a bordo están ahora permanentemente muertos.

central-command-sender = Comando Central

soviet-commissariat-sender = Comisariado del Pueblo Soviético

rev-headrev-count =
    { $initialCount ->
        [one] Solo hubo un líder de la revolución:
       *[other] Hubo { $initialCount } líderes de la revolución:
    }
rev-headrev-name-user = [color=#5e9cff]{$name}[/color] ([color=gray]{$username}[/color]) reclutó a {$count} {$count ->
    [one] contratista
    *[other] contratistas
}

rev-headrev-name = [color=#5e9cff]{$name}[/color] reclutó a {$count} {$count ->
    [one] contratista
    *[other] contratistas
}

## Deconverted window

rev-deconverted-title = ¡Reconvertido!
rev-deconverted-text =
    Como el último agente soviético ha muerto, la gloriosa revolución ha terminado.

    Ya no eres revolucionario. Vuelves a ser un contratista de NanoTrasen.

    Cualquier otra fechoría queda registrada y es sancionable. Así que pórtate bien.

rev-deconverted-rule = Recordatorio: Según la Regla 3 de las reglas del servidor, [bold][color=#a4885c]los revolucionarios reconvertidos olvidan lo que ocurrió mientras tenían el cerebro lavado.[/color][/bold]

rev-deconverted-ruletext = Tu personaje puede enterarse de lo que pasó mediante investigación y rol posterior, pero no debería poder recordar que fue revolucionario ni ninguna de las acciones que cometió en nombre de la revolución.

rev-deconverted-confirm = Entendido
