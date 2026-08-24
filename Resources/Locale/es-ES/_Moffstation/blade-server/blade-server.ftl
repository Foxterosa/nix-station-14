moff-blade-server-rack-window-title = Rack de servidores blade
moff-blade-server-rack-window-footer-flavor = FIRMWARE DEL DISPOSITIVO © 2125 NANOSOFT

moff-blade-server-rack-slot-status = Ranura {$index}: {$content}

moff-blade-server-rack-slot-entity-unknown = desconocido
moff-blade-server-rack-slot-empty = vacía

moff-blade-server-rack-slot-eject = Expulsar
moff-blade-server-rack-slot-insert = Insertar
moff-blade-server-rack-slot-power-toggle = Encender/Apagar

moff-blade-server-rack-slot-locked-fail = ¡Está bloqueado!
moff-blade-server-rack-slot-whitelist-fail = ¡Eso no encaja!

moff-blade-server-rack-examine-empty = No contiene [color=#1f8ab2]blades[/color].
moff-blade-server-rack-examine-single = Solo contiene {$slot}.
moff-blade-server-rack-examine-multiple-start = Contiene
moff-blade-server-rack-examine-multiple-slot-line = - {$slot}
moff-blade-server-rack-examine-slot = [color=#1f8ab2]{ CAPITALIZE($name) }[/color] en la ranura {$index}
moff-blade-server-rack-examine-distant =
    Contiene [color=#1f8ab2]{$numBlades} { $numBlades ->
        [1] blade
        *[other] blades
    }[/color], pero desde esta distancia no puedes distinguir { $numBlades ->
        [1] cuál es
        *[other] cuáles son
    }.

moff-blade-server-frame-incompatible-board = Esta placa parece incompatible con el armazón...
moff-blade-server-board-compatible-hint = Se puede usar para fabricar un [color=#1f8ab2]servidor blade[/color]
