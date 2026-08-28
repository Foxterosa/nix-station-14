delivery-recipient-examine = Esto está pensado para { $recipient }, { $job }.
delivery-already-opened-examine = Esto ya se ha descubierto.
delivery-earnings-examine = Entregar esto traerá [color=yellow]{ $spesos }[/color] dinero al complejo.
delivery-recipient-no-name = Sin Nombre
delivery-recipient-no-job = Desconocido
delivery-unlocked-self = Has desbloqueado { $delivery } con tu huella dactilar.
delivery-opened-self = Has abierto { $delivery }.
delivery-unlocked-others = { CAPITALIZE($recipient) } desbloqueado { $delivery } { POSS-ADJ($possadj) } huella digital.
delivery-opened-others = { CAPITALIZE($recipient) } abrió { $delivery }.
delivery-unlock-verb = Desbloquear
delivery-open-verb = Abierto
delivery-slice-verb = Abierto
delivery-teleporter-amount-examine =
    { $amount ->
        [one] It contains [color=yellow]{$amount}[/color] delivery.
        *[other] It contains [color=yellow]{$amount}[/color] deliveries.
    }
delivery-teleporter-empty = { $entity } vacío.
delivery-teleporter-empty-verb = Recoged los paquetes
# modifiers
delivery-priority-examine = This is a [color=orange]priority {$type}[/color]. You have [color=orange]{$time}[/color] left to deliver it to get a bonus.
delivery-priority-delivered-examine = This is a [color=orange]priority {$type}[/color]. It got delivered on time.
delivery-priority-expired-examine = This is a [color=orange]priority {$type}[/color]. It ran out of time.

delivery-fragile-examine = This is a [color=red]fragile {$type}[/color]. Deliver it intact for a bonus.
delivery-fragile-broken-examine = This is a [color=red]fragile {$type}[/color]. It looks badly damaged.

delivery-bomb-examine = This is a [color=purple]bomb {$type}[/color]. Oh no.
delivery-bomb-primed-examine = This is a [color=purple]bomb {$type}[/color]. Reading this is a bad use of your time.
