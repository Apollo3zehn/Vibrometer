import axi_vip_pkg::*;
import axi_4_axi_vip_0_0_pkg::*;

`timescale 1ns / 1ps

module axi_4_master_tb();
    
    reg aclk    = 0;
    reg aresetn = 0;
    
    axi_4_wrapper DUT
    (
        .aclk(aclk),
        .aresetn(aresetn)
    );
    
    // Declare agent
    axi_4_axi_vip_0_0_slv_mem_t      slv_mem_agent;
    
    initial begin
        slv_mem_agent = new("master vip agent", DUT.axi_4_i.axi_vip_0.inst.IF);
        slv_mem_agent.set_agent_tag("Slave VIP");
        slv_mem_agent.set_verbosity(400);
        slv_mem_agent.start_slave();
    end

    initial begin
        #150ns;
        aresetn                     = 1;
    end

    always begin
        #4ns;
        aclk                        = !aclk;
    end

    initial begin
        #2000ns;
        $finish;
    end

endmodule