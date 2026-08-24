analysis-console-menu-title = Consola de análisis de amplio espectro Mark 3
analysis-console-server-list-button = Servidor
analysis-console-extract-button = Extraer puntos

analysis-console-info-no-scanner = ¡No hay ningún analizador conectado! Conecta uno usando un multitool.
analysis-console-info-no-artifact = ¡No hay ningún artefacto presente! Coloca uno sobre la plataforma para ver la información de sus nodos.
analysis-console-info-ready = Sistemas operativos. Listo para escanear.

analysis-console-no-node = Selecciona un nodo para verlo
analysis-console-info-id = [font="Monospace" size=11]ID:[/font]
analysis-console-info-id-value = [font="Monospace" size=11][color=yellow]{$id}[/color][/font]
analysis-console-info-class = [font="Monospace" size=11]Clase:[/font]
analysis-console-info-class-value = [font="Monospace" size=11]{$class}[/font]
analysis-console-info-locked = [font="Monospace" size=11]Estado:[/font]
analysis-console-info-locked-value = [font="Monospace" size=11][color={ $state ->
    [0] red]Bloqueado
    [1] lime]Desbloqueado
    *[2] plum]Activo
}[/color][/font]
analysis-console-info-durability = [font="Monospace" size=11]Durabilidad:[/font]
analysis-console-info-durability-value = [font="Monospace" size=11][color={$color}]{$current}/{$max}[/color][/font]
analysis-console-info-effect = [font="Monospace" size=11]Efecto:[/font]
analysis-console-info-effect-value = [font="Monospace" size=11][color=gray]{ $state ->
    [true] {$info}
    *[false] Desbloquea nodos para obtener información
}[/color][/font]
analysis-console-info-trigger = [font="Monospace" size=11]Disparadores:[/font]
analysis-console-info-triggered-value = [font="Monospace" size=11][color=gray]{$triggers}[/color][/font]
analysis-console-info-scanner = Escaneando...
analysis-console-info-scanner-paused = En pausa.
analysis-console-progress-text = {$seconds ->
    [one] T-{$seconds} segundo
    *[other] T-{$seconds} segundos
}

analysis-console-extract-value = [font="Monospace" size=11][color=orange]Nodo {$id} (+{$value})[/color][/font]
analysis-console-extract-none = [font="Monospace" size=11][color=orange] Ningún nodo desbloqueado tiene puntos restantes para extraer [/color][/font]
analysis-console-extract-sum = [font="Monospace" size=11][color=orange]Investigación total: {$value}[/color][/font]

analyzer-artifact-extract-popup = ¡La energía centellea sobre la superficie del artefacto!
