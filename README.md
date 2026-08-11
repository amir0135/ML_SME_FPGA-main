# ML_SME_FPGA

Hardware implementation of a **feedforward ensemble neural network** on FPGA, built with [SME (Synchronous Message Exchange)](https://github.com/sme-projects/sme).

Each mathematical operation in the network — matrix multiplication, transpose, PReLU, sigmoid, softplus, reduction — is implemented as an independent SME process that can be simulated in C# and then exported to VHDL for synthesis. The reference software model lives in [`Feedforward-Network`](https://github.com/amir0135/Feedforward-Network); this repository reproduces it in hardware and validates the two against each other.

Master's thesis work — see the [project hub](https://github.com/amir0135/Master-Thesis-Project-Hub) for the full context.

## Why SME

SME describes hardware as a network of processes communicating over typed buses, with a globally synchronous clock. That gives you two things at once: a C# simulation you can debug with ordinary tooling, and a VHDL export that is cycle-accurate to what you simulated. Every directory below is one process (or process group) in that network.

## Repository layout

| Directory | Role |
|---|---|
| `Deflib/` | Shared library — bus definitions, parameters, RAM processes, activation functions, simulation scaffolding |
| `Load_data/` | Reads weight and input CSVs from `Data/` into on-chip memory |
| `Data/` | Pre-trained weights (`W0`, `Wz`, `Wr`, `z_scale`, PReLU slopes) and test inputs, exported from the PyTorch model |
| `Matmul/` | Matrix multiplication process |
| `Transpose/` | Matrix transpose process |
| `Feedforward/` | Top-level feedforward pass wiring the primitives together |
| `HzHr/` | Computes the `hz` / `hr` branches (dual PReLU activations) |
| `RZ/`, `z_r/` | Gate (`z`) and magnitude (`r`) reduction stages |
| `mulmin_sig/` | Fused multiply–minimise with sigmoid activation |
| `Sigmoid/` | Sigmoid activation |
| `Softplus/` | Softplus activation |
| `Clamp/` | Output clamping to ±`max_predict` |
| `Mean/` | Ensemble averaging across sub-networks |
| `sum_lastaxis/` | Reduction along the last axis |
| `SME_raw/` | Minimal SME examples used as building blocks and sanity checks |
| `zz/` | Scratch / experimental process |

Each project is a standalone .NET console application with its own `.csproj`, a `Processes.cs` defining the SME processes, and a `simulation.cs` wiring up the test harness.

## Prerequisites

- [.NET SDK 6.0+](https://dotnet.microsoft.com/download)
- Optional, for synthesis: [GHDL](https://github.com/ghdl/ghdl) or a vendor toolchain (Vivado, Quartus) to build the exported VHDL

## Running a simulation

Each directory can be run on its own:

```bash
git clone https://github.com/amir0135/ML_SME_FPGA-main.git
cd ML_SME_FPGA-main

# Run the full feedforward pass
dotnet run --project Feedforward

# Or exercise a single primitive
dotnet run --project Matmul
dotnet run --project Sigmoid
```

Simulations read their weights from `Data/`, so run from the repository root.

## VHDL export

SME emits VHDL as part of the simulation run when the exporter is enabled in `simulation.cs`. Generated output lands in the project's `output/` directory alongside a testbench, ready to feed into synthesis.

## Validating against the software model

The CSVs in `Data/` are the same ones consumed by the PyTorch model in [`Feedforward-Network`](https://github.com/amir0135/Feedforward-Network). Running both on the same input lets you diff the outputs and confirm the fixed-point hardware path stays within tolerance of the floating-point reference.

## Tech stack

C# · .NET · SME · VHDL

## License

MIT — see [LICENSE](LICENSE).
