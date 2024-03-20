# SDG Systems

A recommended way to develop systems that act on groups of components.

Systems in SDG are completely optional, and are decoupled from the Entity-Component aspect.

The pattern to use them is, create an EntityGroup in the
constructor, then iterate over the objects during Update or Draw.
