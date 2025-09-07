## 

# NeoScavenger Mod Making Guide

Written by Layarion; mod author of Overhaul and Devkit.  
Version 0.02, beta.

**Important\!** This is a new skill you are about to learn, and it will require dedication. Expect to set aside at least two weeks to get up to speed with the rest of the NeoScavenger modding community. You'll need to put in time, and have a willingness to learn.

That being said, I made this guide to expedite that process. Let me know if the guide failed you in some way, so I can improve it for others. Oh, and even after this guide google will still be your friend.

## Prerequisites

**First**, make sure you read the cover letter.

**Second**, if you don't know how to install and play existing mods, you *need to know*. So let's do a very quick rundown. 

* Download the .zip  
* Extract and copy the contents.  
* Paste everything in the games main folder, *the one that has NEOScavenger.exe in it*. *Which I'll call the "root folder" from now on.*  
* Paste the mods getmods.php file in the root folder.  
* Play.

If that didn't work, you might want to open the getmods.php file and make sure \&strModURL has the right file path. If not, fix it and move on.

For more detail on any of the above, ask in Discord or look around.

**Third**, give yourself a crash course on XML documents. It'll help if you know what nodes, tags, elements, and attributes are. I personally recommend [XML Basics \-- for Beginners](https://youtu.be/nyk8QO08grM).

It's not hard once you learn and get used to it.

The ghist of it is, the game has a lot of modifiable ["blocks of code"](https://i.imgur.com/zbIXVrB.png), that's made up of smaller "[1-line commands](https://i.imgur.com/Vhj9nyR.png)", and we're able to change the "[details](https://i.imgur.com/IDZ8kqP.png)" of those commands via the .xml documents. So being able to tell what is just xml gibberish that must be included in every line we make, a command that the game must follow, or details of that command; is important to know.  
Note1: To be clear, you can't change the name of the commands, like "strName", but you can change the details.

**Fourth**, you don't need to look into it much right now, but when you finally start digging into the xml, learn what an XSD or XML Schema is, and use it. It's mentioned in more detail at the Concepts sub-section of the Introduction.

## 

## Introduction

### Concepts

Let's begin with things that you need to think about sooner rather than later.

**Fail faster, iterate often**. Not everything can be modded in the game. There are limits, and you'll want to test those limits. That means do as small a sample as possible and see if it works before you commit to a plan. Do this to save yourself the time of implementing an idea 2-50 times in code, only to find out it doesn't work like you thought it would and you just wasted hours of work.

**Document your work**. No matter the reason, when you finally make a working mod, and that specifically means it "doesn't crash the game on load"; then take this link to heart: [Mod authors, making a better tomorrow](https://bluebottlegames.com/content/mod-authors-making-better-tomorrow). [Example1](https://i.imgur.com/iamuFkt.png) "it would be easier to just remake it than continue on" is a real comment from a mod author and is why you take this part seriously.

**Keep backups**. When you change something, and confirm it works, just hit Copy on the entire mod folder and put it somewhere convenient. This way, when it doesn't work, you can go back to a working version or even use that to compare the two sets and see what went wrong. Don't do an hour long stint without loading the game in-between; to ensure everything still works. [Example1](https://lh5.googleusercontent.com/jt3ZgLuUuIQWzfbYkbtgyxK3DueYGMDUsyMDTYC-lgvYpdB4u4H_R2sWetxZ5D0UFHhCNpKjdEK7fdL8EbkpD-Kc0hKGu8EDVFH-VZM3Zid73HkG2nJrOacCZH9MHxihSdcdTJ_G)

**Use a XML schema**. Also known as a XSD. *These help you spot mistakes that break your mod*. I used to mod without it, and now that I learned about it, *this is something I cannot stress enough, use it.* This link explains why, and how in more detail. [Schema for NeoScavenger](https://bluebottlegames.com/content/tool-xsd-xml-schema).

### What makes a mod?

getmod.php files, and XML file sets are the essence of a mod.

The getmod file tells the game about your mod, and the xml files have the code that makes the magic happen.

#### Getmods.php

The getmods.php [Example1](https://i.imgur.com/guPKBfD.png) [Example2](https://i.imgur.com/LReFEnG.png) file tells the game about the mods you want to use. When the game loads, it reads the php file to know where the mods are, which ones to load, what order to load them in, if they replace existing data, and if they have an internal name. Without it, the game doesn't even know your mods exist.

#### XML sets

The xml files makeup the actual mod itself, and this is where you'll spend most of your time. You'll see xml files named like [this example](https://i.imgur.com/JLQ4DmF.png), and what you're seeing is different "table object types'' of code being quarantined into their own file. Each file has one or hundreds of ["blocks of code"](https://i.imgur.com/zbIXVrB.png) that all start with whatever TableName it is at the top.

Each ObjectType is doing a task, and through those lines you directly control what that task is by tweaking them. *That* is where we live; in the land of learning what each line does, and how to make these tasks do what we want.

Not all mod authors like separating their ObjectTypes into their own file though, for various reasons. In the links above you seen [segmented xml files](https://i.imgur.com/w52I6uV.png), but when we want each ObjectType to be lumped into one file, it *has* to be called neogame.xml. [Like so](https://i.imgur.com/zTgztfR.png). I personally prefer one file, and the split approach was a function added to the game years later.

Note1: When using the single neogame.xml approach, *don't* put it in a data folder. When using the segmented approach, *do* put it in a data folder, and *don't* add that data folder to the getmod URL paths.

Note2: Mods are made up of the folders that hold XML *sets*, which means in the NSE mod, the NeoScavExtended folder isn't the core of the mod. Instead, NSExtended and NSEoverride are two folders that each hold a different mod. It's sensible to think of it as one mod overall though, since both sets need each other to accomplish their goal. Just understand that the game sees it as two mods.

Note3: Neither xml approach is better than the other, pro's and con's to each, but none have a performance hit on the game.

Last thing about xml files. There are two types of XML sets. *Types*, I'm not talking about approaches to the organization like we just talked about [Example1](https://i.imgur.com/JLQ4DmF.png). I'm also not talking about the [ObjectType](https://i.imgur.com/RHfaWgF.png) either. I'm talking about "is this set of XML files creating a new, *unique* thing in the game? or is it changing an already existing thing in the game?"

One set will change how an existing block of code for a unique gun, illness, scene, or feature works in the game. The other set, will add totally new blocks of code that add a unique gun, illness, scene, or feature to the game. Why am I wording it like that? You can take a block of code from the games vanilla files \- such as the sling, Copy it, and paste it in your mod.

Then you could rename it from sling, to "ThotDestroyer". Now, depending on if the code you just pasted in is in a VanillaOverride xml set *(xml files that change existing data)* or in an AddOn xml set *(xml files that just add new items)* it'll do two completely different things.

If you pasted that in a VanillaOverride, any player that installs your mod will have their existing sling, and all future slings, renamed to ThotDestroyer. If you pasted that in an AddOn xml set, their existing sling will remain untouched, *and* two different slings will be attainable in the game. One that's named sling, and the other named ThotDestroyer. The AddOn set adds new items to the game, no matter how similar they are to other code. You could even forego changing the name, and just have two slings named *sling* in the game. That would suck though, because you wouldn't be able to tell them apart. As one mod might do something special with the vanilla sling, like make it shoot lasers...but the player wouldn't know if they're using *your* sling, or the *game's* sling, and the other Mod would only work for the vanilla sling.

Note1: Unless you modified the other mod and your mod correctly, then both slings could shoot lasers.

Sooo that xml bit was a lot to take in. I feel like the AddOn vs VanillaOverride needs extra help in the "wtf did I just read" department, so one more try just for good measure.

Let's say every unique idea, feature, scene, or item in the game had its own number. This is different from a name, because people can have the same name. Instead, this is more like a unique number assigned at birth that no one else is ever allowed to have.

Now let's say you made a mod, that was literally just copying and pasting the vanilla sling, and putting it's code in your mod. Not just any part of your mod though, you put it in an AddOn xml set. This sling of yours, is an exact clone, you didn't change a thing.

Then we'll say the vanilla sling is identified as number 001, and your sling is identified as number 305*.* They're both named *sling*. When in the game, if the player were to put both slings side by side and mouse over them, they wouldn't be able to tell them apart. They *look* identical, but the game knows their true nature, and doesn't think they're the same in any way, shape, or form. The sling on the left is ItemType 001, and the sling on the right is ItemType 305\. They aren't the same type of item, even if the code under the hood looks identical.

Now let's rewind a bit, and instead of pasting the same code in an AddOn XML set, we pasted it in a VanillaOverride xml set.

You no longer have two distinct items, the sling on the left is ItemType 001, and the sling on the right is also ItemType 001\. 305 doesn't exist. Then let's say in your VanillaOverride, you renamed the sling to ThotDestroyer, but changed nothing else...you only changed the name displayed on the players tooltip. Both slings are still the same ItemType, because the game deleted the vanilla code related to that ObjectType, and replaced it with your version. So when the player tries to load in the vanilla sling, they can't, the code literally got pasted over. Only ThotDestroyer can be found now, and all past, present, and future slings will be named ThotDestroyers. ThotDestroyers everywhere.

VanillaOverrides quite literally take *your* ObjectType blocks of code, and *paste them over* the game's vanilla ObjectType blocks of code in the XML sets... As in, if you didn't create any xml files and just modified the xml in the root data folder directly, you'd be doing the *exact* same thing that a VanillaOverride is doing.

Note1: The reason we *don't* edit the root datafolder/xml files directly is many. Don't do it if you're not going to completely trash that file and replace it with a fresh, unmodified copy soon after experimentation is over.

### Recap

Whhhheeeewww. Work was done, but my lord we *need* a recap that's neatly packaged.

So, things you should understand by now:

1. Protecting your time and keeping your code easy to understand is as important as the work itself.  
2. Mods are made up of two logically divisible things. The getmod file, and XML sets stored in folders.  
3. XML sets are made up of blocks of code that represent different ObjectTypes. Each ObjectType fulfills many *tasks* that the game will do when it's time. You can change what that task is doing, by modifying the individual lines inside the ObjectType.  
4. XML sets can be stored/organized in two ways, a single neogame.xml file, or several xml files in a data folder; named after the ObjectType they hold.  
5. XML sets can either be VanillaOverrides, or AddOns.

You don't need to genuinely understand point 1 to finish this guide, but when finished, please take the entirety of point 1 seriously.

As for the rest of the points, these will help you **a lot** when going through the rest of this guide, and in your day to day life as a mod author/creator.

## 

## Nitty Gritty Time

From here on out we'll be picking up the pace; using some of the words I defined in the Intro. Digging deeep into the bowls of modding. It'll probably get harder from here on in.

### Getmods.php

It's back, it's black, and we're here to slay this beast. We didn't actually talk about this much in the intro, just mentioned that it's a mod manager really.

Well, it's time to open it up and explain *everything* about this file. Let's start with the vanilla getmod file, and have a look.

![][image1]

In the image above, at the very tip top of it, we see the file path, this is the game's *root folder*, it's not a sub-folder of the game. Mods can have getmod files tucked away in their own folders for re-use and safe keeping, but the game never looks at those. It only ever looks to the one in the root folder for mod information. Oh, and it *has* to be named *getmods.php*, case sensitive, or it won't work.

In the tab just below the filepath, you can see that this is indeed the getmods.php file. Below that, is something alien and arbitrary.

Let's pull it out of the image and focus on it.

| nRows=0 |
| :---- |

Everything to the left of the **equals** is asking how many mods the game should load and look for. Everything to the right is the answer to that question, none in this case. We are telling the game there are no mods it should load. So if I could rename it, i'd make it look like this:

| TotalNumberOfMods=NumberProvidedByTheModAuthor |
| :---- |

| TotalNumberOfMods=none |
| :---- |

This means the game is done with this file, and moves on with life. Doing nothing with any mod related business.

Now let's pick a getmod file from a real mod. Which mod? How about we start with the popular one, NeoScavenger Extended.

| nRows=2 \&strModName0\=NSE\&strModURL0\=Mods/NeoScavExtended/NSExtended \&strModName1\=0\&strModURL1\=Mods/NeoScavExtended/NSEoverride |
| :---- |

We have some new goop to demystify now, but you can think of nRows as a "fill in the blank question and answer". You can think of everything you see in the above as a question, or an answer. The *answers* to those questions are highlighted in orange to make it easier to read.

So nRows, strModName, and strModURL are questions that need answered. You, the mod author, manually types in the answers to those questions. The game doesn't guess or try to fill in these answers, and neither does the getmod. The red number is special, it's both a question *and* an answer wrapped into one, but more on that soon. Think of it as an answer to a question no one bothered to explicitly ask, but you knew it still needed answered.

The getmod file is asking questions, and Chiko (mod author of NSE) manually typed-in those answers.

so the goop actually four *new* questions:

1. Is this xml set a VanillaOverride set, or an AddOn set?  
2. If it's an AddOn, what is it's internal name?

   Note1: VanillaOverrides don't need a name, so they don't get one. In computers, a zero means "off", or in this use-case, it means there is no name for this xml set because it's a VanillaOverride. They don't need one because the game can figure out that it's not an AddOn from the lack of a name. So you can identify an Override by it's lack of a name. Though, if you decide to say "zero" is it's **name**, rather than a lack of a name, it won't hurt anything, and some mod authors call them zero files for that reason. I think VanillaOverride is more intuitive though.

   Note2: Internal Names are not something the player ever sees or cares about, that's there for mod authors and the game to figure out how to talk to different xml sets.

3. Which xml set should be loaded into RAM first?  
4. Which folder is this xml set located in?

So there's alot to take in here, and just like before; let's break this down by renaming things.

| nRows=2 \&strModName0\=NSE\&strModURL0\=Mods/NeoScavExtended/NSExtended \&strModName1\=0\&strModURL1\=Mods/NeoScavExtended/NSEoverride |
| :---- |

| nRows=2 \&strModName0\=NSE\&strModURL0\=Mods/NeoScavExtended/NSExtended \&strModName1\=0\&strModURL1\=Mods/NeoScavExtended/NSEoverride |
| :---- |

| TotalNumberOfMods=NumberProvidedByTheModAuthor \&WhatIsXMLSetsNameRAMLoadOrder\=Name\&WhatFolderIsSetInRAMLoadOrder\=XmlSetsFolderLocation \&WhatIsXMLSetsNameRAMLoadOrder\=Name\&WhatFolderIsSetInRAMLoadOrder\=XmlSetsFolderLocation |
| :---- |

| TotalNumberOfMods=TwoSets \&WhatIsXMLSetsNameFirst\=NSE, AddOn set. \&WhatFolderIsSetInFirst\=Mods/NeoScavExtended/NSExtended \&WhatIsXMLSetsNameSecond\=Doesn't have one, Zero. VanillaOverride set. \&WhatFolderIsSetInSecond\=Mods/NeoScavExtended/NSEoverride |
| :---- |

Hopefully that clears things up, but one more time.

* strModName is asking if this set adds new content (if it's an AddOn set), and if so, it needs a name, what is it?  
* strModURL is asking what folder this set is located in.

  Note1: The final folder holding the xml set can be located in the root folder, without even being in a master subfolder, [Example1](https://i.imgur.com/J2o5OjJ.png). You can also put those two folders [here](https://i.imgur.com/dXa1x3g.png) without having them in a master subfolder. However, please *don't do that*. The first example clutters the root and mods folder, and the second example comes into play if you make "merged mod packs" that uses other mods without a master folder to hold all the ones you used, you can overwrite the standalone versions of the mods you used and cause trouble for lots of people. So please, use a master subfolder, and also put that master subfolder in the Mods folder.

* The number you see immediately after "ModURL" or "ModName" is the load order. It starts at 0, thus the NSE AddOn set will be the first set loaded into RAM.

  Note1: This is important if another xml set calls on something in the NSE AddOn set. If the NSE AddOn set doesn't load before the other set that needs it, the game will fail to load.

* nRows is just asking "what's the total count of xml sets I'm loading?"

And that's it. If you're wondering what the "str" you see in "strModName" and "strModURL" is, it's a programmers shorthand for "string". If that's confusing, don't worry about it because it means little to us in the end. The "&" tells the game that this is the start of a new question.

A few other things, more than one AddOn or VanillaOverride can be in the getmod, and you don't have to have equal amounts of either, such is the case with the DevKit mod. Which has two AddOn sets, and no VanillaOverrides.

Okay, let's do a bigger example, to really nail the point home.

This example comes from the popular, *Mighty(mini) Mod of Doom*.

| nRows=5 \&strModName0\=FoD\&strModURL0\=FieldsofDead \&strModName1\=0\&strModURL1\=FieldsZero \&strModName2\=SP\&strModURL2\=SagesPages \&strModName3\=MoD\&strModURL3\=M(m)MoD \&strModName4\=0\&strModURL4\=MoDZero |
| :---- |

I'm not going to rename and break things apart this time, I think that's been covered thoroughly. However, I will point out that:

1. Yes, the folders holding the xml sets are right at the root folder, and that the xml sets aren't even packaged into a master subfolder, but we can fix that ourselves pretty easily.  
     
   Just create the master subfolder, move them into it, and then move the master into the mods folder. Finally, type the new file path in the getmod URL sections.  
2. As mentioned, you don't need to have any matching pairs of AddOns and Overrides. Just by counting the "zero" names, I can see that this mod has two VanillaOverride sets, and I see three named AddOn sets.  
3. Something else that we haven't mentioned yet, and isn't a required rule, but *is nice*...the folders that are holding VanillaOverride, he tacked "Zero" at the end of that folders name. This wasn't by accident, he did that to remind himself what type of set that folder was.  
   1. You'll see that the NS modding community has a couple of different ways of implementing this "convention". This author marked the Override folder by tacking on "zero", but other authors might use "over" "0" "override" and I personally use "VanillaOverride".  
      1. That last one is long, but I have my reasons, and it's not for aesthetics. When you've been away for a while "zero" and "over" can just leave you more confused than anything else. So I like being more explicit in this case. Helps me remember without having to "look things up"  
   2. Now is a good time for me to mention that AddOn or AddOn sets are terms I invented because there is no naming convention for non-override xml sets. "well it doesn't have zero or override in it's folder name" isn't enough. The best names for a thing is one that explains what it does.  
        
      Conventions help us, and avoiding ambiguity without having to dig into it saves us time, and sometimes mistakes. I also don't think it's uncommon to rename something often, as you write a piece of code, it often becomes more clear to you what this thing is doing. Sometimes things you've made end up different from what you started with. So don't be bothered if you don't like the original name you picked for your mod, or the tag I mentioned in [Mod authors, making a better tomorrow](https://bluebottlegames.com/content/mod-authors-making-better-tomorrow). Just rename it as you go.

Ok I think that covers the getmod file. It is a mod manager that tells the game how many xml sets, where they are, what type of set they are, and if an AddOn type then what is it's name, and what order to load those sets into RAM. 

Even if you don't make a mod, this knowledge will help you as a mod user, but it's critical to understand as a mod author. Besides using it to make your own mods, you also use it to combine or merge mods together. 

It's time to move on. Mods are made of two things, the getmods file, and xml sets. We are done with the first part.

### 

### The actual folder

This section focuses on the xml sets, and everything that makes a mod. I haven't or have hardly mentioned the other things that might be found in the same folder as these xml sets, because there isn't much. Really it's just image files, and you won't be able to use those without an understanding of the xml first.

So I already mentioned in the Prerequisites that the xml is made of blocks of code that I'm calling ObjectTypes. Then the individual lines can be thought of as tasks that the game will do. I'll stick with "task" for my noob audience's sake, but you can also call them "functions" or "variables" if you have any programming background. Third thing is, you can edit the details of those tasks to make them do different things, and that's how we end up creating something new in the game, or how we make a mod in general.

Well it's time to get deeper into it. Let's start from the top. You have the xmlFolder, it holds the xml(not talking about any master sub-folders here). It can either have a single neogame.xml file, or a data folder holding one or several .xml files.

Then, when you open one of the vanilla xml files, you're greeted [with this](https://i.imgur.com/QOpNCrN.png). *insert hopeless music here*, no don't play that\! We can fix this.

So I need to reiterate something important here; you need to do some research and really understand everything in the Prerequisites section. It has important things like: [XML Basics \-- for Beginners](https://youtu.be/nyk8QO08grM), and if you like to view things from more than one source, then try this [written guide](https://www.sitepoint.com/really-good-introduction-xml/) as well. It's more detailed, but not as concise as the other.

Also important, but still too soon to talk about, is [Schema for NeoScavenger](https://bluebottlegames.com/content/tool-xsd-xml-schema). I mentioned it in the intro, it's important to mention again. Keep this in the back of your mind when you start modding. It'll save you hours of bug fixing common mistakes.

#### Breaking down the .xml file

Using [the above image](https://i.imgur.com/QOpNCrN.png) as a reference, and assuming you remember everything from the video, line one is optional and means nothing to us.

Starting at line two is a comment that ends at line 11\. Comments are not code, the computer doesn't see them. They're only there for us to either a) remove a piece of code without having to delete it, or b) write something more legible for ourselves to understand without breaking the code. [Example1](https://i.imgur.com/TBqVQBc.png). Worth mentioning: [https://stackoverflow.com/a/14650451](https://stackoverflow.com/a/14650451) as a way to nest comments. It allows you to bypass [this issue](https://i.imgur.com/vzDRfaX.png). Solution [example](https://i.imgur.com/uF4OuKl.png).

line 13 is our first element, named pma\_xml\_export. This is our "root", or if you prefer, our first parent node. As a mod author of NS we don't need to understand whatever it's saying, all we need to do is just make sure we have our database element nested inside it.

Lines 14-42, completely ignore that. it's not anything we're using nor will ever use, and I already provided something similar to it anyway. Don't even need to have it in your code.

43, more comments. denoting the start of the code we actually care about.

46, this is the database element. your Table object types(getting to that soon) needs to have this as their parent. You'll only have one pma and database parent per file.

48, ok now we're getting somewhere. Remember that up until this point you can pretty much leave all of the above out of your xml files, except for the pma and database parents. This, is the Table element. It has a name attribute. You can change what's in the double quotes as long as what you're changing it to is one of [these predefined names](https://i.imgur.com/JLQ4DmF.png). You choose the name depending on what you need, however, if you're using the multiple xml file approach, don't name the table something other than the name of the file. oh, and *everything* is case sensitive in this guide. Don't change the name to Attackmodes, because it's not the same as attackmodes with no capital letters.

So to recap, comments are tools that we can completely ignore of they're not serving us, or even delete them. (please use them to make it more obvious what the code is doing though, like I already shown above). A Lot of what you see can just be completely cut-out, and Table elements need to be stored inside of the single database element, and the database element needs to be stored inside the single root pma element. If that's hard to visualize, I simplified it in [this image](https://i.imgur.com/piKROr4.png). You don't need all that goop we seen with the schema and xml at the tip top of the other document.

You're more than halfway to making a mod now, almost the final stretch even. Just reaffirm your knowledge of what the table elements are, and then all that's left to understand is the smaller child elements that I call tasks. Now's a good time to cover the meaning of each TableType AKA ObjectType.

There are 24 table types: 

* attackmodes \- handles what ammo/charges and battlemoves a weapon might use.   
* barterhexes \- locations the player can buy and sell items on the ground at.  
* battlemoves \- If it happens during a battle and has a graphic on the bottom left of the screen, it needs/is a battlemove.   
* camptypes \- When you press R in-game, you'll see these to the right.  
* chargeprofiles \- This is where you define What ItemType consumes What ItemType. The ItemType flashlight Consumes Electricity ItemType, Gun Items consume Bullet Items.  
* conditions \- These basically increase/decrease almost any stat in the game. Philips health, how an item works...basically these *do* things to other things. So much to study here, you'll have to look at it in the xml.  
* containertypes \- Defines what type of container an object can fit in, and defines what type of object a container can hold.  
* creatures \- Determines the stats, gear, ID, and faction of any entity.  
* creaturesources \- Turns a single coordinate on the map into a spawn point for whatever creature you set it to. [More info here.](https://bluebottlegames.com/comment/16052#comment-16052)  
* datafiles \- the stuff you can get from hacking and unlocking software in the game.  
* dmcplaces \- places beyond the wall that you can interact with.  
* encounters \- events that can do alot of things, including give or take stuff.  
* encountertriggers \- when and how to trigger an event.  
* factions \- controls AI behaviour, or how AI will treat you.  
* gamevars \- controls a couple of global variables like weather, start date, and so on.  
* headlines \- controls what you'll see on newspapers.  
* hextypes \- what is on a hex, it's movement cost, applied conditions, light levels, lots of stuff, even some combat stuff.  
* ingredients \- used in recipesitemprops \- used to mark an ItemType as a type of ingredient or multiple ingredients.  
* itemtypes \- these do alot, almost everything you do routes back to these in some way.  
* maps \- Just guessing… 1\) lets you decide each and every hex type at each coordinate. 2\) the game might randomize between predefined sets...  
* recipes \- if you wanna make something in the crafting screen, this is what you need. Controls not only what items turn into what, but if they're used, and even if they should be hidden (like bad water)  
* treasuretables \- one of the other great pillars of mod authors in this game, anything that needs spawned, created, has a loadout, even stores, this thing basically handles any kind of possible loot table/reward/scavenge choices you can think of.

They each contain their own unique child elements or tasks. Some of these things I've never had to touch as an author, and to understand what each table and child element does takes loads of time. So I and perhaps many others, do not have all the answers and they may not even been accurate when we do have them. You'll have to do some research yourself and fill in the blanks, but in the References section I am trying to answer what I can, when I can. This should give you a good start though. If you learn the answers, share them in a comment so I can add them later.

**alright\!** we've left that "just getting to know you" phase, and enter the part that'll let you make mods. Table/ObjectTypes are your lifeblood, and so are their child elements. You want to add a new gun to the game? Make an ItemType, create some images to go with the ItemType, then an Attackmode so the player can pick different ammo, create a chargeprofile for that ammo if you're using custom ammo, then have that profile consume an ammo ItemType, make a battlemove for the gun itemtype, an ItemType for the Battlemove itself if it's a custom battlemove, and then finally start handling how the player will attain the item. Such as modifying the treasuretables, by going into a scavenge treasuretable and adding the guns itemtype to it, or create an TresTable and give a recipe to go with the gun, and have the recipe call on the treasure table.

so that was perhaps just a mess for you to look at, but my point was Object/TableTypes are how code interacts with other code in this game. 

So tasks, or the child elements, functions, variables, whatever you wanna call them. This is where alot of your time will be spent, learning what each one does. I'm running out of steam now, so I'll wrap this guide up soon. In order to learn what most of these do, you have to experiment and use google. In the reference section I was trying to imtimately learn each one, but it's no 5 minute job. If you can figure out the details shoot them to me and I'll add that to that reference link.

Sorry to say, but I've run out of steam. The child element section is so vast that I got depressed thinking about the time it would take to document even half of it. So before I take off, 

### how to google NS modding topics

Google has a few keywords and symbols to help your searches pull up more relevant results to your modding questions. When you do a google search, using the keyword site: let's you lock down results to things that are only from that url.

If you use "exampleWord" double quotation marks, it'll put searches that don't have that exact spelling, phrasing, ordering, and spaces on the bottom of the list.

With this knowledge, if I wanted to see if anyone ever mentioned attack IDs that could poison someone, I might google this:  
site:https://bluebottlegames.com/ "NEO SCAVENGER MODS" poison attack

site:https://bluebottlegames.com/ tells it to only search on the BBG website.

"NEO SCAVENGER MODS" every modding branch in BBG has this at the top of it's page, it's the only way we can narrow results down to the modding forums.

poison attack is what you're currently trying to find. This is the only section you're interested in changing for your needs, and you should leave the previous two alone.

### The XML Schema, or XSD. 

[Read this](https://bluebottlegames.com/content/tool-xsd-xml-schema), and use it. This will save you a lot of trouble. 

## Closing notes

Ended abruptly, but take the Concepts section seriously, and what I have provided will give you a much better start than what I started with, and there is more modding documentation on the BBG site, it's even called "official modding documentation" or something like that. Once you finish this guide, those documents might make more sense. 

Last thing, if you're looking at child elements and you see 0:SomeTableTypeIDNumber or NSE:SomeTableTypeIDNumber the item on the left is a mod call. 0: is telling the game "the source code or original block of this ID exist in the vanilla data folder" and the other says "the source code or original block of this ID exist in this xml AddOn set" and that's why AddOn's need names and Vanilla's don't. If you're physically looking at a block of code inside of an AddOn set, you don't need to use *that* sets name before each ID, only for calls to things outside it's set. Like vanilla files and other xml sets. So if you were in your mods AddOn or Vanilla set, calling on something from the NSE set, you'd need to use NSE:Number, but not YourMod:Number(if in your AddOn set, you would have to if you were in your Vanilla set). That's also when Load Order becomes much more important; would break the game if your vanilla or addon set loaded into RAM before the NSE set did.

Ok good luck, maybe i'll be back to finish this someday. You could add some comments/edits and I'll approve the changes if you want to help.

# 

# References

These are an attempt to compress all the information a mod author might want to know. They're far from being completed though, if a mod author wants to help, hit me up.

[ObjectTypes and Child elements breakdown](https://docs.google.com/document/d/1K_UfYIece_HBUjmB47_NKv8e4paGmW65n7iWNqjZXDI/edit?usp=sharing)

# NotForReaders

## Stuff I took out and might do something with later

## Example time

Ok I think it'll be easier to explain via two examples. 

The **first example** will only be about what happens when the game runs using the popular mod, NeoScavenger Extended. This will help you fix common problems you'll probably run into at least once, and it'll help you make your first mod. [What happens at run. Mod author series.](https://www.youtube.com/playlist?list=PLdSl223_yxmEJtyxxrXGrQBqO_hu6oxe8) Abandoned video series, low on time and money.

The **second example**, we'll actually make a mod from scratch. With the knowledge gained from the first example you'll be able to understand what I'm doing and why.

### First example

Download the NSE mod, and copy the getmod and Mods folder inside it. Paste it at the games root folder. which is the same folder that has NEOScavenger.exe in it.

Congratulations, NSE is installed and should be working. What did we actually **do** though? from a mod authors perspective, **a lot**. More than just throwing files in the games folder, we replaced the Getmod, which means the ModManager is spouting different instructions to the game.

Let's break that down.

#### **Breakdown of the getmod php file**

Remember, what I said in the Intro section about the getmod php, it's a *Mod Manager*. Let's open it now and break-down what that means.

nRows=2  
\&strModName0=NSE\&strModURL0=Mods/NeoScavExtended/NSExtended  
\&strModName1=0\&strModURL1=Mods/NeoScavExtended/NSEoverride

With the NSE mod that's what you'll see when you open the getmod. In the Vanilla game though, you'll actually see this:

nRows=0

What does nRows=0 mean though? well If I renamed it to something more meaningful, it'd look like this

HowManyModsAmILoading=none

The game is asking, on the left, how many mods should it load, and the author says zero (this time). So all that line is, is you saying the Total number of XML sets to load.

## Video Scripts

### WhatHappensAtRun

#### Intro

Over the next few videos. I'm going to explain to you, several different parts of what it takes to run a mod. I'm not really going to explain, what goes into making a fresh mod in this example, but we'll cover installing a mod, the getmod PHP file \- and then touch the subject of XML files \- and the images around them.

it's important you understand \- how this process works overall.      Because it demystifies mod making.

we're going to segway into the next video now, where I sum up the main concerns of a mod author, and the parts that make a mod.

#### What is a mod made of

ok part 1, what do we care about as mod authors.

Short answer is the data folder, getmod PHP file, and the mod folders themselves \- which I haven't added to this fresh install yet.

We care about the data files because anything you want to make, pretty much has to have some loose form of existing to manipulate. What does that mean? Want to make sharks that shoot frickin lasers out of their eyes? open the data folder, dig in, and see if you can work out a way \- to make the existing mechanics do the shiny new thing you want. After getting lost in them, you miiight come out thinking you can take the same blocks of code that create murder hobo crackheads, and give them a new graphic, weapon, and perhaps behavior that is a shark. Spoiler, almost nothing we can do about what decisions an NPC makes. The data files are there to see what has been done, and how we can tweak things to create something new.

okay where were we, um data folder, and now ah...ah the getmod file.

What do WE care about the getmod file? It's kinda hard to sum up, but it's a Mod Manager that tells the game which mods we're loading, where they are, and a few other things I'll explain soon. So soon, that I'll leave it there.

Last thing. The mod folders themselves. Mods are REQUIRED to have two things. a getmod PHP file, and sets XML files. Other things that can, but don't have to, make up a mod, is image files. You know, custom art for just about anything you can see in the game. Oh\! and we can't add new sounds or music to the game. hshhh yeah sorry. So the mod files consist mostly of just XML sets, with a sprinkle of images. Anything that isn't image files themselves, will be done in the XML and thus \- this is where you'll LIVE as a mod author when you're not drawing or briefly managing the getmod file.

That ends the video, and I just want to sum it up one more time. 1\) Study what you can do in the data folder, which has the games default xml file sets. 2\) Manage the getmod file, and 3\) make your own xml sets and images.

as a note, you can lump the study of other mods into point one; seeing what has been done.

oookay, next video, getting nitty gritty with the Getmod file.

#### Installing and the Getmod file

So we covered what a mod is made of, and now we'll dive into the first thing both the mod author and players will run into when doing annnything with a mod. Installing it and it's getmod file.

Before we get into this, I want to mention that this is long and I am sorry, but it's for SCIENCE\!

Using the *very* popular NSE mod, we'll extract it...and paste. Confirms that it works...it does...and now talk about it.

So hopefully all mods install as easily as this one did, all I really had to do was Copy \- and Paste. If you're mildly unfortunate you'll have to open and edit the PHP file to make everything work. That's still a good day if that's all that happens, but it's our job as mod authors to make sure it doesn't get to that.

Having a firm understanding of what's happening behind the scenes will help you ensure this simplicity, as well as avoid problems for yourself when making or merging mods.

So it all starts with the getmod PHP file. Without it, the game doesn't even know your mods exist. In the best of cases, neither you nor the player has to think about it much, and in the worst cases it can crash your game and cause major headaches as an author.

But before we open it up and explain whatTheFuck i'm talking about, I want to point something out that bothers me about the way NSE used the PHP file. You notice that when I extracted and pasted it, the PHP file is gone and can't be found in it's folder here? That means if I install a different mod, but then reinstall this one later on...instead of just grabbing the file from here, I need to relocate the extract or the download that I probably deleted when I was done. Or if I didn't delete them, finding them is annoying when I coulda just grabbed it from you know...HERE.

So think about your end users and all their possible use cases. These mods are for them after all.

Back on topic, opening the getmod and breaking it down piece by piece.

When we pasted in the mod, we did a little more than just throw files in. We changed what info the PHP file feeds the game.

Time to open it up and have a look.

We see NSE has a couple of things going on in here, but let's back it up to vanilla status and see what the default game has. All that's here, is just nRows equals 0\. If I could rename this part, to something more descriptive of what it's doing...

...This is what it'd say, and it reflects what the PHP file is telling the game. On the left of the equals sign, the game is asking how many mods to expect and look for, and on the right is the current answer. Zero, there are zero mods I want loaded.

So, the game is asking how many mods am I loading? and the getmod is saying none.

Fastforward back to the NSE version, and we see that it says "I chiko, mod author of NSE, have two mods I want to load"

now you might be asking "isn't NSE just one mod?"... ehhhh...to us...but not to the game. I'll get back to this, but first…

let me, once again, rename what all this gibblygob means into more meaningful sentences.

...and there. 

I said mods were made of the php file and XML file SETS. A single php file can combine several XML SETS into one seamless install. Each set of XML files has a purpose, and there are two types of xml sets. You can see them here, you have AddOn XMLs, and OverRide XMLs.

Whaaats the difference? In summary, one set changes or replaces existing code in the default game, and the other creates new code that doesn't interfere with the default code. Which, I'll call default code Vanilla from now on because it's easier to say for me.

VanillaOverrides and Addons. This mod is using both, and you can tell if it's an Override just by looking at the internal name. If it has Zero

[image1]: <data:image/png;base64,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>