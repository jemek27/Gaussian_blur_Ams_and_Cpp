import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import re
import sys

def bar_plot_multiple_sizes(df, savePath, kernel_size="", iterations="1"):
    
    size_names = df['Size-name'].unique()
    threads = df['Threads'].unique()

    # Data preparation
    stats = df.groupby(['Threads', 'Size-name']).agg(
        cpp_mean=('Time-cpp-ms', 'mean'),
        cpp_std=('Time-cpp-ms', 'std'),
        asm_mean=('Time-asm-ms', 'mean'),
        asm_std=('Time-asm-ms', 'std')
    ).reset_index()

    # Number of groups (C++ and ASM for each data size)
    num_sizes = len(size_names)
    width = 0.15  # Single column width
    x = np.arange(len(threads))  # Group base positions (Threads)

    cpp_colors = ['#89CFF0', '#4682B4', '#0F52BA']  # Shades of Blue
    asm_colors = ['#FF7F7F', '#FF4500', '#8B0000']  # Shades of Red

    # Creating a chart
    fig, ax = plt.subplots(figsize=(12, 8))

    for i, size in enumerate(size_names):
        
        size_data = stats[stats['Size-name'] == size]

        # Calculate X offset for C++ and ASM
        cpp_offset = -width * num_sizes + width / 2 + i * width
        asm_offset = cpp_offset + width * num_sizes

        # C++
        ax.bar(x + cpp_offset, size_data["cpp_mean"], width,
               yerr=size_data["cpp_std"], capsize=5,
               label=f'C++ {size}', alpha=0.7,
               color=cpp_colors[i % len(cpp_colors)])

        # ASM
        ax.bar(x + asm_offset, size_data["asm_mean"], width,
               yerr=size_data["asm_std"], capsize=5,
               label=f'ASM {size}', alpha=0.7,
               color=asm_colors[i % len(asm_colors)])

    
    ax.set_xlabel('Threads')
    ax.set_ylabel('Time (ms)')
    ax.set_title('Performance Comparison C++ vs ASM')
    ax.set_xticks(x)
    ax.set_xticklabels(threads)
    ax.legend()
    ax.grid(axis='y', linestyle='--', alpha=0.7)

    if kernel_size != "":
        # Adding additional information at the bottom of the chart
        fig.text(0.98, 0.02, f'Kernel size: {kernel_size} Test performed x{iterations}', ha='right', fontsize=10)

    plt.tight_layout()
    joined_names = '_'.join(size_names)
    plt.savefig(f'{savePath}{joined_names.upper()}_dll_times_bar.png') 
    plt.show()

if __name__ == "__main__":
    csvPath = sys.argv[1] 
    filename = sys.argv[2]  
    print(csvPath+filename)

    df = pd.read_csv(csvPath+filename)
    df = df[df['Iterations'] != 1]

    pattern = r'-K(\d+)-I(\d+)'
    match = re.search(pattern, filename)
    if match:
        k_value = match.group(1)  # Value after K
        i_value = match.group(2)  # Value after I

    print(k_value, i_value)
    bar_plot_multiple_sizes(df, csvPath,k_value, i_value)