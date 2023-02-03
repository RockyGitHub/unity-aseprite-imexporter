# unity-aseprite-imexporter
exports aseprite files to spritesheets then imports them into spritedow animations

An example JSON structure is the follows:
```json
{
    "exportCommands": [
        {
            "name": "back-angelic-bow",
            "layers": [
                "Marksman Back Bow/Angelic Bow"
            ],
            "exportPath": "Marksman/Back_Bows/Angelic"
        },
        {
            "name": "back-crimson-bow",
            "layers": [
                "Marksman Back Bow/Crimson Bow"
            ],
            "exportPath": "Marksman/Back_Bows/Crimson"
        },
    ],
    "tags": [
        "Idle",
        "Crouch",
        "Hurt",
    ]
}
```