# ML_SME_FPGA

This repository provides added libraries for implementing a Feedforward Neural Network (FNN) using SME ([Synchronous Message Exchange]([url](https://github.com/sme-projects/sme))) on FPGA. It includes various components required for building, simulating, and optimizing FNN models for FPGA-based implementations.

## Overview

This project aims to streamline the integration of FNNs with SME on FPGA. It provides various simulation scripts, pre-trained weights, and activation functions, specifically optimized for FPGA environments.

The repository is organized into several directories that hold different functionalities, such as matrix multiplication, data loading, and simulation scripts, to facilitate the neural network implementation on FPGA.

## Project Structure

- **Clamp/**: Includes files related to the clamp operation within the neural network.
- **Data/**: Contains the pre-trained weight files and other necessary data for neural network simulation.
- **Deflib/**: Holds various utility scripts, including functions, parameters, and processes needed for the simulation and running of the neural network.
- **Feedforward/**: Implements the feedforward operation of the neural network.
- **HzHr/**: Contains scripts for handling Hz and Hr operations within the neural network model.
- **Matmul/**: Responsible for matrix multiplication processes.
- **Mean/**: Scripts for calculating the mean within the neural network operations.
- **SME_raw/**: Raw scripts used for basic operations in SME.
- **Sigmoid/**: Contains scripts to implement the sigmoid activation function.
- **Softplus/**: Implements the softplus activation function.
- **Transpose/**: Handles matrix transpose operations.
- **mulmin_sig/**: Focuses on operations related to multiplication and minimization combined with sigmoid activation.
- **sum_lastaxis/**: Handles operations related to summing across the last axis.
- **z_r/**: Handles operations related to z_r transformations.
- **zz/**: Implements the zz function used within the neural network model.

## Usage

1. First, download the SME environment from the official SME repository.
2. Clone this repository:
   ```bash
   git clone https://github.com/amir0135/ML_SME_FPGA-main.git

3. Follow the instructions in the corresponding directories to compile and simulate your neural network model.

## Dependencies
- SME (Small Memory Environment)
- An FPGA development environment (e.g., Xilinx Vivado)

## License
This project is licensed under the MIT License. 
