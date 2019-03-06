## Alternate Skins

Prerequisites (0)

>If you just want to add new textures to existing pets,
>add a new texture in (1) and expand the tooltip list in (4), nothing else required.

* "ClassName" excludes the "Proj" suffix, it will be mentioned in the guide when you need to add it
* if the projectile doesn't have "Proj" in its name, you don't need to include it manually, but for things like
classNameType, there should be no "Proj" in it
* in PetPlayer.cs: `public bool ClassName = false;` and `ClassName = false;` in ResetEffects()
* in each step after the first, the place where you need to add stuff is marked via `//ALTERNATE`

***


 (1) Textures

* Name the textures like this: ClassNameProj_number and put them in the same folder as the class

***


 (2) Class File

* Add this just after `public class ... {`:

```csharp
public override string Texture
{
    get
    {
        return "AssortedCrazyThings/Projectiles/Pets/ClassNameProj_0";
    }
}
```
* Add this at the very end:
```csharp
public override void PostAI()
{
    PetPlayer mPlayer = Main.player[projectile.owner].GetModPlayer<PetPlayer>(mod);
    Main.projectileTexture[projectile.type] = mod.GetTexture("Projectiles/Pets/ClassNameProj_" + mPlayer.classNameType);
}
```

* Check with Modder's Toolkit if the hitbox aligns with the texture, if not, set `drawOffsetX/drawOriginOffsetY` accordingly in SetStaticDefaults() (example: YoungWyvern.cs)

***



 (3) PetPlayer File

* Create a byte variable where the alt texture is saved and add it to the Load() and Save() hooks:

```csharp
//name pet texture
public byte classNameType = 0;

[...]

public override TagCompound Save()
{
    return new TagCompound {
        [...]
        {"classNameType", (byte)classNameType},
        [...]

public override void Load(TagCompound tag)
{
    [...]
    classNameType = tag.GetByte("classNameType");
    [...]
```

***


 (4) Register textures, add tooltips in UI/CircleUIConf

* At the end of the class file, add this:
* "Default" is the \_0 texture, "AltName1" is the \_1 texture etc
* It will only register the number of textures specified as tooltips,
so if you have 10 textures but only name 6 tooltips it will only pick the textures from 0 to 5 (as opposed to 0 to 9)

	```csharp
	public static CircleUIConf ClassNameConf()
	{
		List<string> tooltips = new List<string>() { "Default", "AltName1", "AltName2", etc };

		return PetConf("ClassNameProj", tooltips);
	}
	```

***


 (5) Register UI to activate

* ACT.cs, CircleUIStart() and CircleUIEnd() hooks:

CircleUIStart():

* if regular pet: under `if (triggerLeft) //left click`, if light pet: under the `else //right click`:
	* under `if (triggerType == ItemType<VanitySelector>())` at the end of the else if chain, before the `else { return; }`:

		```csharp
		else if (pPlayer.ClassName)
		{
			CircleUI.currentSelected = pPlayer.classNameType;

			CircleUI.UIConf = CircleUIConf.ClassNameConf();
		}
		```

CircleUIEnd():

* if regular pet: under `if (triggerLeft) //left click`, or if light pet: under the `else //right click`:
	* under `if (triggerType == ItemType<VanitySelector>())` at the end of the `else if` chain:

		```csharp
		else if (pPlayer.ClassName)
		{
			pPlayer.classNameType = (byte)CircleUI.returned;
		}
		```

***


 (6) Multiplayer stuff

* ACT.cs:
    * at the very bottom, add classNameType to the list of `PetPlayerChanges`

* PetPlayer.cs:
    * in clientClone(), add `clone.classNameType = classNameType;`
	* in SendClientChanges(), add `else if (clone.classNameType != classNameType) changes = PetPlayerChanges.classNameType;`
	* in SendFieldValues(), add: `packet.Write((byte)classNameType);`
	* in RecvSyncPlayerVanitySub(), add: `classNameType = reader.ReadByte();`
	* in RecvClientChangesPacketSub(), before the `default: //shouldn't get there hopefully`, add:

	```csharp
	case (byte)PetPlayerChanges.classNameType:
		classNameType = reader.ReadByte();
		break;
	```

	* in SendClientChangesPacketSub(), before the `default: //shouldn't get there hopefully`, add:

	```csharp
	case (byte)PetPlayerChanges.classNameType:
		packet.Write((byte)classNameType);
		break;
	```

***


Finally, you can go into these files and search for "//ALTERNATE" to see if you implemented everything (examples included in each instance):
 * PetPlayer.cs: 11 occurences (2 of which are ticked off already if you do it on an existing pet)
 * ACT.cs: 5 occurences (you'll only need three (there are two for left/right click each))
 * UI/CircleUIConf.cs: 1 occurence
 