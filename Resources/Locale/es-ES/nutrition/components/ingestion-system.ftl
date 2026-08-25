
### Interaction Messages

# System

## When trying to ingest without the required utensil... but you gotta hold it
ingestion-you-need-to-hold-utensil = Necesitas tener {$utensil} en la mano para comer eso.

ingestion-try-use-is-empty = ¡{CAPITALIZE(THE($entity))} está vacío!
ingestion-try-use-wrong-utensil = No puedes {$verb} {THE($food)} con {$utensil}.

ingestion-remove-mask = Primero debes quitarte {$entity}.

## Failed Ingestion

ingestion-you-cannot-ingest-any-more = ¡Ya no puedes {$verb} más!
ingestion-other-cannot-ingest-any-more = ¡{CAPITALIZE(SUBJECT($target))} ya no puede {$verb} más!

ingestion-cant-digest = ¡No puedes digerir {THE($entity)}!
ingestion-cant-digest-other = ¡{CAPITALIZE(SUBJECT($target))} no puede digerir {THE($entity)}!

## Action Verbs, not to be confused with Verbs

ingestion-verb-food = Comer
ingestion-verb-drink = Beber

# Edible Component

-edible-satiated = { $satiated ->
    [true] {" "}Ya no sientes que puedas { $verb } más.
  *[false] {""}
}

edible-nom = Ñam. {$flavors}{ -edible-satiated(satiated: $satiated, verb: "comer") }
edible-nom-other = Ñam.
edible-slurp = Sorbo. {$flavors}{ -edible-satiated(satiated: $satiated, verb: "beber") }
edible-slurp-other = Sorbo.
edible-swallow = Te tragas { THE($food) }.{ -edible-satiated(satiated: $satiated, verb: "tragar") }
edible-gulp = Glup. {$flavors}
edible-gulp-other = Glup.

edible-has-used-storage = No puedes {$verb} { THE($food) } con un objeto guardado dentro.

## Nouns

edible-noun-edible = comestible
edible-noun-food = comida
edible-noun-drink = bebida
edible-noun-pill = pastilla

## Verbs

edible-verb-edible = ingerir
edible-verb-food = comer
edible-verb-drink = beber
edible-verb-pill = tragar

## Force feeding

edible-force-feed = ¡{CAPITALIZE(THE($user))} está intentando obligarte a {$verb} algo!
edible-force-feed-success = ¡{CAPITALIZE(THE($user))} te obligó a {$verb} algo! {$flavors}{ -edible-satiated(satiated: $satiated, verb: $verb) }
edible-force-feed-success-user = Alimentas a {THE($target)} con éxito.
