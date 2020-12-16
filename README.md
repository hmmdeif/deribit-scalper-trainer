# Deribit Scalper Trainer

Connects directly to deribit api but all positions and calculations are done in-memory, meaning that no trades are ever actually executed against the exchange. This is purely
for practicing and testing strategies for scalping.

N.B. Mark prices stabilise over the first 30 seconds on startup as the calculations are based on the last 30 seconds EMA.

## Why WPF?

Because I felt like it. I'll port it to MAUI later so people on non-Windows can run it.

## Example

![App Image](https://github.com/hmmdeif/deribit-scalper-trainer/blob/main/example.png?raw=true)