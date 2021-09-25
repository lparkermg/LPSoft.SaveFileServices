# LPSoft.SaveFileServices
Generic service for saving and loading saves.

## Usage

###  Service Setup

Initialising the included `SaveService` with a maximum slot value will set an array of the provided data type to the amount of slots provided in the constructor. For example the following will setup a new service with 5 slots of the `string` data type:

```
var service = new SaveService<string>(5);
```

### Saving Data

To save the current slots in the service you will need to run `Save("path/to/save/file")` with the file you wish to save to. Running this will allow you to save the data to file on some kind of persistant storage.

### Loading Data

To load previously saved slots in the service you will need to run `Load("path/to/save/file)` with the file you wish to load from. Running this will allow you to load an existing set of saves from a previously saved file.

### Getting a slot

To get a slot from the service you will need to run `Get(index of the slot)` this will return the slot data for usage.

### Setting a slot

TO set a slot in the service you will need to run `Set(index of the slot)` this will overwrite the existing slot data.

## Planned features/changes

These will be implemented at some point in the future:

- Callback to confirm setting data
- Callback to confirm loading data
- Enforcing slot size
    - Currently it is possible to load a file that would increase or decrease the slot size.
- Wrap exceptions from loading and saving in service specific exceptions.
