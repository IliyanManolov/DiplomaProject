# Properties explanation
In an attempt to reduce the bandwidth usage caused by appplication logs the properties have been shortened as much as possible. This list will show the abbreviation and the property behind it.

All values are truncated to 8001 bytes due to OpenSearch not allowing longer values to be passed and used for filtering through the UI.
The logs format is also used when logging to the Console of the application

- `ts` - timestamp of when the logs was created (! NOT received)
- `v` - `verbosity`, severity level of the log. Possible values: `Debug`, `Information`, `Warning`, `Error`, `Fatal`. By default the application only emits logs of `Information` and higher
- `msg` - the templated message of the log (e.g. "Code {code} is already used")
- `prop` - a collection of objects with `name` and `value` properties. Holds the values corresponsing to the template values in logs ("{code}" in the example above)
  - `name` - name of the template value
  - `value` - the value itself from the log
- `propLong` - same as the above, cannot be filtered on using the UI due to size being longer than the maximum allowed by OpenSeach
- `tr` - tracking id of the request, unique and allows displaying all logs originating within a single request
- `ctx` - context, the .NET class where the log originates from
- `err` - Exception message
- `errType` - Exception Type (e.g. NullReferenceException, etc.)
- `st` - Exception stack trace
- `innerEx` - inner exceptions flattened to a single collection. Each item contains the properties described above - `err`, `errType`, `st`