# Flag Generator Library

Everything to make a flag is in here.

## Notes on source graphics

  - flag_layers.svg has layers used in flags
  - crests / contains crests that can be placed over the flag



## flag_layers.svg format

This file has a layer naming convention that instructs the code how to use a layer.

The underscore _ is reserved as a separator character.
Spaces and '-' are allowed.

The top level layers are chosen with equal weighting.
  -Any hierarchical layers, are consider mods the the above layer (eg, disc with star etc) 
    - They are pulled in with equal probability if the parent layer is chosen.
    - 
     

name_orientation_occupies_mods

  - name
    - name for descriptive purposes
    - will be used in generating a description for the flag
    - use heraldric / vexillological terms (https://en.wikipedia.org/wiki/Glossary_of_vexillology)
    - eg 'stripe', 'disc'
  mods
    - ni   no inersect
    - f    flipable
    - fa   works with itself flipped, flip to any colour
    - fs   works with itself flipped, flip to same colour only
    - fd   works with itself flipped, never flip to same colour