# Dialogue System Manual
## Preface
This is a Unity Dialogue System designed to be abstract and simple to use for
the average Unity Project while maintaining a high level of flexibility. It 
handles parsing strings and text files in a custom language explained in 
detail further in this document.

## Dialogue Language
The dialogue Parsing language is node based. All dialogue is located along a 
node. To deal with parsing the language into nodes. Special characters should be
set in the dialogue parsing language. Listed below are the default values but
these can be changed in settings

**All are followed by a string identifier**
| Syntax | Description |
|-|-|
|`- NodeName`|Give a node a name to reference|
|`> FunctionName (param1, param2)`|Executes function with given name and params|
|`-> Option Text`|A single of options to display|
| `\| GotoNodeName` | Changes execution to given node|
|`# Comment`|Comment within the text|
|`Person: Hello World`|A line in the dialogue system with a speaker|
|`Hello World!`|A line in the dialogue system without a speaker|
|`[ID]`| Used to reference dialogue elements programatically|

With these simple commands dialogue can be easily created and modified in a text
editor without having to go programatically set it up.
### Terminating
If a node has no other lines (another node following a node will take priority
and the current nodes lines will end before that next node definition) execution
will terminate on the dialouge system. 

### Identification
The ID `[ID]` is an optional parameter that can be added after a command call
to give the dialogue line an additional identifier. These identifiers have
global scope in the dialogue system and can be referenced to modify lines at
runtime.

### Options
Options will provide a set of choices that can be made in a dialogue system.
The Options can be given an id: `[OptionsId]` the main purpose for this is to 
enable / disable options if needed.

**4 spaces or 1 tab (cannot be mixed)**  can be used to create branching paths 
within the dialogue tree. Execution will continue below the option once the 
option has exited. Options can also be nested.

### Multiple commands
Multiple commands are supported for executed on a single line. The parser will 
treat them as separate lines when the dialogue system is actually read however 
this syntax, can improve clarity in cases.
```
-> Go to park > Park(true) | GoNode # Executes function and goes to GoNode
-> Don't go > Park(false) | StayNode # Executes funciton and goes to StayNode
```

### Escape
Special characters can be escaped using `\` To escape an escape character type
`\\`

### Example
```
- Hello 
Person: Hello there
-> Hi!
    Person: Hey there!
-> Hows it going?
    Person: Its going good, how are you?
    -> Bad
        Person: Sorry to hear that!
    -> Good
        Person: Glad you're doing well!
Person: Hope Have a good day!
You: You too!
```
### Examples for control flow
```
# Compacted written format

- Start
-> 1 # Goes to 2
    -> 2 # Goes to 8
        D:8
        -> 5 # goes to D:11
        -> 6 # goes to D:11
        -> 7 # goes to D:9
            D:9 # goes to D:11
    -> 3 # Goes to D:10
        D:10 # Goes to D:11
    D:11 # Goes to D:12
-> 4 # Goes to D:12
D:12

# Underlying substitution

- Start
-> 1 | N1
    - N1
    -> 2 | N2
        - N2
        D:8
        -> 5 | N3
            - N3
            | N4
        -> 6 | N5
            - N5
            | N4
        -> 7 | N6
            - N6
            D:9
            | N4
        - N4
        | N7
    -> 3 | N8
        - N8
        | N7
    - N7 
    D:11
    | N9
-> 4 | N9
- N9
D:12
-> 13 | 15
-> 14 | 15
- 15
```

# Formatted Reader
Within text blocks we also might want the ability to animate / modify specific 
characters. So that we can specify specific blocks of text which are animated to
break up the animation within strings, there is a small formatting language used
which lets us identify and select the text.
### Formatting
Id is specified after the dollar sign `$` and any formatting we want is applied
to the text within the parentheses.
```
$id(Formatted string)
```
### Example
```
// Dialogue line is returned to the Animation Reader
// Dailogue line speaker: "Tom" text: "${bold}(Hello) there!"
// Animation Reader picks up and passes a formatts "Hello there!"

Tom: $bold(Hello) there!
```