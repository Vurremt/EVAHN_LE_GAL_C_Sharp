# EVAHN_LE_GAL_C_SHARP_PROJECT

## Projet Tp Noté C# .NET
### Fait par Evahn Le Gal
### Rendu le 04/02/2024

## Tâches réalisées

- Application fonctionnelle de gestion d'arborescences de contacts
- Connaitre le dossier courant avec entête dans le terminal
- Chaque dossier connait sa profondeur dans l'arborescence et son dossier parent, le parent de Root est lui même (comme dan cmd)
- Gestion du format des emails saisis
- Selection du dossier courant et déplacement dans l'arborescence
- Suppression possible de répertoires et de contacts
- Gestion des noms de dossiers et contacts identiques dans un même dossier
- Lien console-application par listener et partie graphique très facilement implémentable (voir section Console-Application)
- Sérialisation dans des fichiers externes, situé dans un repertoir nommé "Appli_c_sharp_Evahn_LE_GAL" dans MyDocuments de l'ordinateur (crée au lancement de l'application la première fois)
- Sérialisation en XML (.xml) et Binaire (.bin) au choix avec le design patern Factory
- Gestion de presque toutes les erreurs possibles, dont les noms de fichiers inexistants, les clés de cryptage fausses, les commandes invalides, etc.
- Cryptage de contenu de la sérialisation par CryptoStream
- Demande de la clé de cryptage lors de la sauvegarde/récupération et hashage de la clé

## Console-Application

Le lien entre l'activité de l'application et l'affichage est clairement découpé et les deux sont bien isolés. La classe EntriesConsole reçoit des instructions (ici sous formes de strings) et appelle les méthodes necessaires de l'application. L'application renvoie quant à elle des résultats à EntriesConsole, qui les communique par l'intermédiaire de listeners à DisplayConsole qui se charge de l'afficher dans le bon format en console. Les listeners sont abonnés à des events de la forme (string, element). Le string est l'instruction de ce qui doit se passer à l'écran, tandis que l'élément apporte des détails ou des objets pour l'affichage.

Ainsi, on peut de manière très simple passer d'une application console à une partie graphique, il suffit de remplacer la classe DisplayConsole et d'implémenter les actions pour chacune des instructions possibles. L'implémentation de ces actions peut être très simple (comme une ligne à afficher en console) ou plus complexe dans une partie graphique, il n'y aura qu'un seul fichier à changer sans pour autant modifier l'application elle même. Idem, il n'y a que la fonction "recupInstruction()" à changer dans EntriesConsole pour récupérer les actions de l'utilisateur, sans avoir à modifier tout le programme.

## Listes des commandes :

- display : affiche la structure en partant du dossier courant
- display all : affiche la structure à partir du Root
- create folder : Demande le nom du dossier à créer
- create contact : Demande le nom de famille, puis le prenom, puis l'adresse mail (qui doit etre conforme), la société et un choix de lien avec l'utilisateur
- delete folder : Demande le nom du dossier à supprimer
- delete contact : Demande le nom de famille et le prenom du contact à supprimer
- cd : mettre en argument un chemin d'accès, comme dans cmd ( ./ et ../ et Root/ sont supportés)
- save xml : Demande le nom du fichier .xml dans lequel sauvergarder l'arborescence (par serialisation xml)
- save binary : Demande le nom du fichier .bin dans lequel sauvergarder l'arborescence (par serialisation binaire)
- load xml : Demande le nom du fichier .xml depuis lequel charger une arborescence
- load binary : Demande le nom du fichier .bin depuis lequel charger une arborescence
- clear : clear le terminal
- help : Ouvre la fenetre d'aide des commandes
- quit : Fermer l'application sans sauvegarder

## Choix Techniques

- Utilisation de listeners et d'event pour la communication entre l'application et l'interface :
- - **Raison 1** : Permet d'envoyer des instructions, ce qui rend indépendants les deux programmes, on peut donc modifier le pogramme sans changer l'interface ou alors apsser d'une interface console à une interface graphique sans toucher à l'application elle-même.
- - **Raison 2** : Permet de pouvoir rajouter facilement d'autres focntionnalités et d'autres applications en parallèle de notre application actuelle car tout est en asynchrone
- - **Raison 3** : Code bien plus lisible, au lieu d'un gros bloc application, l'application n'envoit qu'une requete simple, et le traitement est fait dans un autre fichier, le code st donc beaucoup moins chargé et mieux découpé
 
- Utilisation de Aes pour le cryptage car c'est très simple de gérer des flux cryptés.

- Utilisation d'un Factory pour une encapsulation supplémentaire et un code plus lisible

- Classe FolderTemp qui est une copie de Folder mais sans parent, pour ne pas avoir d'erreurs lors de la serialisation (notamment XML), car cela provoquait des problèmes de dependances cycliques

## Difficultés Rencontrées

- Grosses difficultés avec la cryptographie, notamment dû au fait qu'utiliser des FileString rajoutait des caractères à la fin des fichiers cryptés, rendant l'extraction impossible (très difficile de s'en rendre compte car fichier illisible). Utilisation de MemoryStream qu'on injecte dans un fichier qui a permit de regler ce problème
- Cryptographie toujours, avec la taille d'encodage de iv, il a fallu passer par du hachage SHA256 pour regler le problème
- Problèmes aussi avec la de-serialisation XML, la serialisation marchait, mais grosses difficultés à regler les problèmes de dé-serialisation
