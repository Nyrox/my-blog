module Pair

let mapFirst mapping pair =
    (mapping (fst pair), snd pair)

let mapSecond mapping pair =
    (fst pair, mapping (snd pair))