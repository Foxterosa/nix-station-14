### Locale for wielding items; i.e. two-handing them

wieldable-verb-text-wield = Empuñar
wieldable-verb-text-unwield = Dejar de empuñar

wieldable-component-successful-wield = Empuñas { THE($item) }.
wieldable-component-failed-wield = Dejas de empuñar { THE($item) }.
wieldable-component-successful-wield-other = { CAPITALIZE(THE($user)) } empuña { THE($item) }.
wieldable-component-failed-wield-other = { CAPITALIZE(THE($user)) } deja de empuñar { THE($item) }.
wieldable-component-blocked-wield = { CAPITALIZE(THE($blocker)) } te impide empuñar { THE($item) }.

wieldable-component-no-hands = ¡No tienes suficientes manos!
wieldable-component-not-enough-free-hands = {$number ->
    [one] Necesitas una mano libre para empuñar { THE($item) }.
    *[other] Necesitas { $number } manos libres para empuñar { THE($item) }.
}
wieldable-component-not-in-hands = ¡{ CAPITALIZE(THE($item)) } no está en tus manos!

wieldable-component-requires = ¡{ CAPITALIZE(THE($item)) } debe empuñarse!

gunwieldbonus-component-examine = Esta arma tiene mejor precisión al empuñarla.

gunrequireswield-component-examine = Esta arma solo puede dispararse al empuñarla.
