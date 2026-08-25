anomaly-component-contact-damage = ¡La anomalía te abrasa la piel!

anomaly-vessel-component-anomaly-assigned = Anomalía asignada al receptáculo.
anomaly-vessel-component-not-assigned = Este receptáculo no está asignado a ninguna anomalía. Prueba usar un escáner sobre él.
anomaly-vessel-component-assigned = Este receptáculo está asignado actualmente a una anomalía.

anomaly-particles-delta = Partículas delta
anomaly-particles-epsilon = Partículas épsilon
anomaly-particles-zeta = Partículas zeta
anomaly-particles-omega = Partículas omega
anomaly-particles-sigma = Partículas sigma

anomaly-scanner-component-scan-complete = ¡Escaneo completo!

anomaly-scanner-ui-title = escáner de anomalías
anomaly-scanner-no-anomaly = No hay ninguna anomalía escaneada.
anomaly-scanner-severity-percentage = Severidad actual: [color=gray]{$percent}[/color]
anomaly-scanner-severity-percentage-unknown = Severidad actual: [color=red]ERROR[/color]
anomaly-scanner-stability-low = Estado actual de la anomalía: [color=gold]En decadencia[/color]
anomaly-scanner-stability-medium = Estado actual de la anomalía: [color=forestgreen]Estable[/color]
anomaly-scanner-stability-high = Estado actual de la anomalía: [color=crimson]En crecimiento[/color]
anomaly-scanner-stability-unknown = Estado actual de la anomalía: [color=red]ERROR[/color]
anomaly-scanner-point-output = Producción de puntos: [color=gray]{$point}[/color]
anomaly-scanner-point-output-unknown = Producción de puntos: [color=red]ERROR[/color]
anomaly-scanner-particle-readout = Análisis de reacción a partículas:
anomaly-scanner-particle-danger = - [color=crimson]Tipo de peligro:[/color] {$type}
anomaly-scanner-particle-unstable = - [color=plum]Tipo inestable:[/color] {$type}
anomaly-scanner-particle-containment = - [color=goldenrod]Tipo de contención:[/color] {$type}
anomaly-scanner-particle-transformation = - [color=#6b75fa]Tipo de transformación:[/color] {$type}
anomaly-scanner-particle-danger-unknown = - [color=crimson]Tipo de peligro:[/color] [color=red]ERROR[/color]
anomaly-scanner-particle-unstable-unknown = - [color=plum]Tipo inestable:[/color] [color=red]ERROR[/color]
anomaly-scanner-particle-containment-unknown = - [color=goldenrod]Tipo de contención:[/color] [color=red]ERROR[/color]
anomaly-scanner-particle-transformation-unknown = - [color=#6b75fa]Tipo de transformación:[/color] [color=red]ERROR[/color]
anomaly-scanner-pulse-timer = Tiempo hasta el próximo pulso: [color=gray]{$time}[/color]

anomaly-gorilla-core-slot-name = Núcleo de anomalía
anomaly-gorilla-charge-none = No tiene ningún [bold]núcleo de anomalía[/bold] adentro.
anomaly-gorilla-charge-limit = Le [color={$count ->
    [3]green
    [2]yellow
    [1]orange
    [0]red
    *[other]purple
}]{$count ->
    [one]queda {$count} carga
    *[other]quedan {$count} cargas
}[/color].
anomaly-gorilla-charge-infinite = Tiene [color=gold]cargas infinitas[/color]. [italic]Por ahora...[/italic]

anomaly-sync-connected = Anomalía acoplada correctamente
anomaly-sync-disconnected = ¡Se perdió la conexión con la anomalía!
anomaly-sync-no-anomaly = No hay ninguna anomalía al alcance.
anomaly-sync-examine-connected = Está [color=darkgreen]acoplado[/color] a una anomalía.
anomaly-sync-examine-not-connected = [color=darkred]No está acoplado[/color] a ninguna anomalía.
anomaly-sync-connect-verb-text = Acoplar anomalía
anomaly-sync-connect-verb-message = Acopla una anomalía cercana a {THE($machine)}.
anomaly-sync-disconnect-verb-text = Desacoplar anomalía
anomaly-sync-disconnect-verb-message = Desacopla la anomalía conectada de {THE($machine)}.

anomaly-generator-ui-title = Generador de anomalías
anomaly-generator-fuel-display = Combustible:
anomaly-generator-cooldown = Enfriamiento: [color=gray]{$time}[/color]
anomaly-generator-no-cooldown = Enfriamiento: [color=gray]Completo[/color]
anomaly-generator-yes-fire = Estado: [color=forestgreen]Listo[/color]
anomaly-generator-no-fire = Estado: [color=crimson]No listo[/color]
anomaly-generator-generate = Generar anomalía
anomaly-generator-charges = {$charges ->
    [one] {$charges} carga
    *[other] {$charges} cargas
}
anomaly-generator-announcement = ¡Se ha generado una anomalía!

anomaly-command-pulse = Hace pulsar una anomalía objetivo
anomaly-command-supercritical = Hace que una anomalía objetivo entre en estado supercrítico

# Flavor text on the footer
anomaly-generator-flavor-left = La anomalía puede aparecer dentro del operador.
anomaly-generator-flavor-right = v1.1

anomaly-behavior-unknown = [color=red]ERROR. No se puede leer.[/color]

anomaly-behavior-title = Análisis de desviación de comportamiento:
anomaly-behavior-point = [color=gold]La anomalía produce el {$mod} % de los puntos[/color]

anomaly-behavior-safe = [color=forestgreen]La anomalía es extremadamente estable. Pulsaciones extremadamente raras.[/color]
anomaly-behavior-slow = [color=forestgreen]La frecuencia de las pulsaciones es mucho menor.[/color]
anomaly-behavior-light = [color=forestgreen]La potencia de las pulsaciones está considerablemente reducida.[/color]
anomaly-behavior-balanced = No se detectaron desviaciones de comportamiento.
anomaly-behavior-delayed-force = La frecuencia de las pulsaciones se reduce mucho, pero su potencia aumenta.
anomaly-behavior-rapid = La frecuencia de las pulsaciones es mucho mayor, pero su fuerza está atenuada.
anomaly-behavior-reflect = Se detectó un recubrimiento protector.
anomaly-behavior-nonsensivity = Se detectó una reacción débil a las partículas.
anomaly-behavior-sensivity = Se detectó una reacción amplificada a las partículas.
anomaly-behavior-invisibility = Se detectó distorsión de las ondas de luz.
anomaly-behavior-secret = Interferencia detectada. Algunos datos no se pueden leer
anomaly-behavior-inconstancy = [color=crimson]Se detectó inconstancia. Los tipos de partículas pueden cambiar con el tiempo.[/color]
anomaly-behavior-fast = [color=crimson]La frecuencia de pulsación está muy aumentada.[/color]
anomaly-behavior-strenght = [color=crimson]La potencia de pulsación está considerablemente aumentada.[/color]
anomaly-behavior-moving = [color=crimson]Se detectó inestabilidad de coordenadas.[/color]
anomaly-secret-admin = [color=red](ERROR)[/color]
